using System.Dynamic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using GameplayAbilitySystem.AttributeSystem.Components;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Systems
{
    [UpdateInGroup(typeof(AttributeUpdateSystemGroup))]
    [UpdateBefore(typeof(AttributeModifierCollectionSystem<,,>))]
    public class AttributeUpdateSystem<TAttributeValues, TInstantAttributeModifier, TDurationalAttributeModifier, TJobExecutable> : SystemBase
    where TAttributeValues : struct, IComponentData, IAttributeData
    where TDurationalAttributeModifier : struct, IComponentData, IAttributeModifier
    where TInstantAttributeModifier : struct, IComponentData, IAttributeModifier
    where TJobExecutable : struct, IAttributeExecute<TAttributeValues, TInstantAttributeModifier, TDurationalAttributeModifier>
    {
        EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
        public static Entity CreatePlayerEntity(EntityManager dstManager, TAttributeValues defaultAttributes)
        {
            var attributeArchetype = dstManager.CreateArchetype(typeof(TAttributeValues), typeof(TDurationalAttributeModifier), typeof(TInstantAttributeModifier));
            var entity = dstManager.CreateEntity(attributeArchetype);
            dstManager.SetComponentData(entity, defaultAttributes);
            return entity;
        }

        private EntityQuery m_InstantAttribute;
        private EntityQuery m_DurationAttribute;

        protected override void OnCreate()
        {
            m_InstantAttribute = GetEntityQuery(typeof(TAttributeValues), typeof(TInstantAttributeModifier));
            m_DurationAttribute = GetEntityQuery(typeof(TAttributeValues), typeof(TDurationalAttributeModifier));
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var durationalAttributes = new DurationalAttributeUpdateJob()
            {
                AttributeDataHandle = GetComponentTypeHandle<TAttributeValues>(false),
                DurationalAttributeModifierDataHandle = GetComponentTypeHandle<TDurationalAttributeModifier>(true),
                Executable = default
            };

            var instantAttributes = new InstantAttributeUpdateJob()
            {
                AttributeDataHandle = GetComponentTypeHandle<TAttributeValues>(false),
                InstantAttributeModifierDataHandle = GetComponentTypeHandle<TInstantAttributeModifier>(true),
                Executable = default
            };

            Dependency = instantAttributes.ScheduleParallel(m_InstantAttribute, Dependency);
            Dependency = durationalAttributes.ScheduleParallel(m_DurationAttribute, Dependency);

        }

        [BurstCompile]
        struct DurationalAttributeUpdateJob : IJobChunk
        {
            public TJobExecutable Executable;
            public ComponentTypeHandle<TAttributeValues> AttributeDataHandle;
            [ReadOnly] public ComponentTypeHandle<TDurationalAttributeModifier> DurationalAttributeModifierDataHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<TAttributeValues> attributeValuesChunk = chunk.GetNativeArray(AttributeDataHandle);
                NativeArray<TDurationalAttributeModifier> durationalAttributeModifiersChunk = chunk.GetNativeArray(DurationalAttributeModifierDataHandle);
                Executable.CalculateDurational(attributeValuesChunk, durationalAttributeModifiersChunk);
            }
        }

        [BurstCompile]
        struct InstantAttributeUpdateJob : IJobChunk
        {
            public TJobExecutable Executable;
            public ComponentTypeHandle<TAttributeValues> AttributeDataHandle;
            [ReadOnly] public ComponentTypeHandle<TInstantAttributeModifier> InstantAttributeModifierDataHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<TAttributeValues> attributeValuesChunk = chunk.GetNativeArray(AttributeDataHandle);
                NativeArray<TInstantAttributeModifier> instantAttributeModifiersChunk = chunk.GetNativeArray(InstantAttributeModifierDataHandle);
                Executable.CalculateInstant(attributeValuesChunk, instantAttributeModifiersChunk);
            }
        }

    }
}
