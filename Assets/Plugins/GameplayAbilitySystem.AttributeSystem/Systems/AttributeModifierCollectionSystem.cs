using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Systems
{
    [UpdateInGroup(typeof(AttributeUpdateSystemGroup))]
    [UpdateBefore(typeof(AttributeUpdateSystem<,,,>))]
    public class AttributeModifierCollectionSystem<TAttributeModifier, TGameplayAttributesModifier, TComponentTag> : SystemBase
    where TAttributeModifier : struct, IComponentData, IAttributeModifier
    where TGameplayAttributesModifier : struct, IComponentData, IGameplayAttributeModifier<TAttributeModifier>
    where TComponentTag : struct, IComponentData
    {
        public static EntityArchetype CreateModifierArchetype(EntityManager dstManager)
        {
            return dstManager.CreateArchetype(typeof(TGameplayAttributesModifier), typeof(GameplayEffectContextComponent), typeof(TComponentTag));
        }
        public static Entity CreateAttributeModifier(EntityManager dstManager, TGameplayAttributesModifier modifier, GameplayEffectContextComponent context)
        {
            var attributeArchetype = CreateModifierArchetype(dstManager);
            var entity = dstManager.CreateEntity(attributeArchetype);
            dstManager.SetComponentData(entity, modifier);
            dstManager.SetComponentData(entity, context);
            return entity;
        }

        private EntityQuery m_AttributeModifiers;
        private EntityQuery m_AttributesGroup;
        protected override void OnCreate()
        {
            m_AttributeModifiers = GetEntityQuery(typeof(TGameplayAttributesModifier), typeof(GameplayEffectContextComponent), typeof(TComponentTag));
            m_AttributesGroup = GetEntityQuery(typeof(TAttributeModifier));
        }

        protected override void OnUpdate()
        {
            if (m_AttributeModifiers.CalculateEntityCount() == 0)
            {
                Dependency = new ResetAttributeModificationsForPlayer()
                {
                    AttributeModifierValuesHandle = GetComponentTypeHandle<TAttributeModifier>(false),
                    EntitiesHandle = GetEntityTypeHandle()
                }.ScheduleParallel(m_AttributesGroup, Dependency);
                return;
            }

            NativeMultiHashMap<Entity, TGameplayAttributesModifier> AttributeModifiersNMHM = new NativeMultiHashMap<Entity, TGameplayAttributesModifier>(m_AttributeModifiers.CalculateEntityCount(), Allocator.TempJob);
            Dependency = new CollectAllAttributeModifiers()
            {
                AttributeModifiersNMHMWriter = AttributeModifiersNMHM.AsParallelWriter(),
                GameplayAttributeModifierHandle = GetComponentTypeHandle<TGameplayAttributesModifier>(true),
                GameplayEffectContextHandle = GetComponentTypeHandle<GameplayEffectContextComponent>(true)
            }.ScheduleParallel(m_AttributeModifiers, Dependency);
            Dependency = new MapAttributeModificationsToPlayer()
            {
                AttributeModifierValuesHandle = GetComponentTypeHandle<TAttributeModifier>(false),
                AttributeModifierCollection = AttributeModifiersNMHM,
                EntitiesHandle = GetEntityTypeHandle()
            }.ScheduleParallel(m_AttributesGroup, Dependency);
            // Now write to the attributes
            AttributeModifiersNMHM.Dispose(Dependency);
        }

        [BurstCompile]
        struct CollectAllAttributeModifiers : IJobChunk
        {
            public NativeMultiHashMap<Entity, TGameplayAttributesModifier>.ParallelWriter AttributeModifiersNMHMWriter;
            [ReadOnly] public ComponentTypeHandle<TGameplayAttributesModifier> GameplayAttributeModifierHandle;
            [ReadOnly] public ComponentTypeHandle<GameplayEffectContextComponent> GameplayEffectContextHandle;
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

        [BurstCompile]
        struct ResetAttributeModificationsForPlayer : IJobChunk
        {
            public ComponentTypeHandle<TAttributeModifier> AttributeModifierValuesHandle;
            [ReadOnly] public EntityTypeHandle EntitiesHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var attributeModifierValuesChunk = chunk.GetNativeArray(AttributeModifierValuesHandle);
                for (var i = 0; i < chunk.Count; i++)
                {
                    TAttributeModifier attributeModifierValue = new TAttributeModifier();
                    attributeModifierValuesChunk[i] = attributeModifierValue;
                }
            }
        }

    }
}