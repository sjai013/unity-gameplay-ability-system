using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Systems
{
    public class AttributeUpdateSystem<TComponentData, TAttributeModifier, TJobExecutable, TGameplayAttributesModifier> : SystemBase
    where TComponentData : struct, IComponentData, IAttributeData
    where TAttributeModifier : struct, IComponentData, IAttributeModifier
    where TJobExecutable : struct, IAttributeExecute<TComponentData, TAttributeModifier>
    where TGameplayAttributesModifier : struct, IComponentData, IGameplayAttributeModifier<TAttributeModifier>
    {
        private EntityQuery m_Attributes;
        private EntityQuery m_AttributeModifiers;
        private EntityQuery m_AttributesGroup;

        protected override void OnCreate()
        {
            m_Attributes = GetEntityQuery(typeof(TComponentData));
            m_AttributeModifiers = GetEntityQuery(typeof(TGameplayAttributesModifier));
            m_AttributesGroup = GetEntityQuery(typeof(TAttributeModifier));
        }

        protected override void OnUpdate()
        {
            var sumAttributes = new AttributeUpdateJob()
            {
                AttributeDataHandle = GetComponentTypeHandle<TComponentData>(false),
                AttributeModifierDataHandle = GetComponentTypeHandle<TAttributeModifier>(true),
                Executable = default
            };
            Dependency = sumAttributes.ScheduleParallel(m_Attributes, Dependency);

            NativeMultiHashMap<Entity, TGameplayAttributesModifier> AttributeModifiersNMHM = new NativeMultiHashMap<Entity, TGameplayAttributesModifier>(m_AttributeModifiers.CalculateEntityCount(), Allocator.TempJob);
            var CollectAttributeModifiersJob = new CollectAllAttributeModifiers()
            {
                AttributeModifiersNMHMWriter = AttributeModifiersNMHM.AsParallelWriter(),
                GameplayAttributeModifierHandle = GetComponentTypeHandle<TGameplayAttributesModifier>(true),
                GameplayEffectContextHandle = GetComponentTypeHandle<GameplayEffectContext>(true)
            };

            Dependency = CollectAttributeModifiersJob.ScheduleParallel(m_AttributeModifiers, Dependency);
            Dependency = new MapAttributeModificationsToPlayer()
            {
                AttributeModifierValuesHandle = GetComponentTypeHandle<TAttributeModifier>(false),
                AttributeModifierCollection = AttributeModifiersNMHM,
                EntitiesHandle = GetEntityTypeHandle()
            }.Schedule(m_AttributesGroup, Dependency);
            // Now write to the attributes
            AttributeModifiersNMHM.Dispose(Dependency);
        }

        [BurstCompile]
        struct AttributeUpdateJob : IJobChunk
        {
            public TJobExecutable Executable;
            public ComponentTypeHandle<TComponentData> AttributeDataHandle;
            [ReadOnly]
            public ComponentTypeHandle<TAttributeModifier> AttributeModifierDataHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var attributeValuesChunk = chunk.GetNativeArray(AttributeDataHandle);
                var attributeModifiersChunk = chunk.GetNativeArray(AttributeModifierDataHandle);
                Executable.Execute(attributeValuesChunk, attributeModifiersChunk);
            }

        }

        struct CollectAllAttributeModifiers : IJobChunk
        {
            public NativeMultiHashMap<Entity, TGameplayAttributesModifier>.ParallelWriter AttributeModifiersNMHMWriter;
            [ReadOnly] public ComponentTypeHandle<TGameplayAttributesModifier> GameplayAttributeModifierHandle;
            [ReadOnly] public ComponentTypeHandle<GameplayEffectContext> GameplayEffectContextHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var attributeModifierChunk = chunk.GetNativeArray(GameplayAttributeModifierHandle);
                var gameplayEffectContextChunk = chunk.GetNativeArray(GameplayEffectContextHandle);
                for (var i = 0; i < chunk.Count; i++)
                {
                    var attributeModifier = attributeModifierChunk[i];
                    var gameplayEffectContext = gameplayEffectContextChunk[i];
                    AttributeModifiersNMHMWriter.Add(gameplayEffectContext.Target, attributeModifier);
                }

            }
        }

        [BurstCompile]
        struct MapAttributeModificationsToPlayer : IJobChunk
        {
            public ComponentTypeHandle<TAttributeModifier> AttributeModifierValuesHandle;
            [ReadOnly] public NativeMultiHashMap<Entity, TGameplayAttributesModifier> AttributeModifierCollection;
            [ReadOnly] public EntityTypeHandle EntitiesHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var attributeModifierValuesChunk = chunk.GetNativeArray(AttributeModifierValuesHandle);
                var entityChunk = chunk.GetNativeArray(EntitiesHandle);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = entityChunk[i];
                    TAttributeModifier attributeModifierValue = new TAttributeModifier();
                    if (AttributeModifierCollection.TryGetFirstValue(entity, out var modifier, out var iterator))
                    {
                        modifier.UpdateAttribute(ref attributeModifierValue);
                        while (AttributeModifierCollection.TryGetNextValue(out modifier, ref iterator))
                        {
                            modifier.UpdateAttribute(ref attributeModifierValue);
                        }
                    }
                    attributeModifierValuesChunk[i] = attributeModifierValue;
                }

            }
        }
    }
}
