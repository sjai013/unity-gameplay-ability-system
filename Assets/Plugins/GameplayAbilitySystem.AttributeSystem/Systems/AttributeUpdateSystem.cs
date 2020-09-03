using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GameplayAbilitySystem.AttributeSystem.Systems
{

    public class AttributeUpdateSystem<TComponentData, TAttributeModifier, TJobExecutable> : SystemBase
    where TComponentData : struct, IComponentData, IAttributeData
    where TAttributeModifier : struct, IComponentData, IAttributeModifier
    where TJobExecutable : struct, IAttributeExecute<TComponentData, TAttributeModifier>
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            m_Query = GetEntityQuery(typeof(TComponentData));
        }

        protected override void OnUpdate()
        {
            // 1. Gather all attribute modifier components
            // EITHER:
            // 2a. Modify value of TAttributeModifier component to reflect the mods
            // OR
            // 2b. Keep those values in memory (NativeStream, NativeMultiHashMap, w/e), and apply directly to attributes
            NativeHashMap<Entity, TAttributeModifier> a = new NativeHashMap<Entity, TAttributeModifier>(m_Query.CalculateEntityCount(), Allocator.TempJob);


            var sumAttributes = new AttributeUpdateJob()
            {
                AttributeDataHandle = GetComponentTypeHandle<TComponentData>(false),
                AttributeModifierDataHandle = GetComponentTypeHandle<TAttributeModifier>(true),
                Executable = default
            };
            this.Dependency = sumAttributes.ScheduleParallel(m_Query, this.Dependency);
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
    }
}
