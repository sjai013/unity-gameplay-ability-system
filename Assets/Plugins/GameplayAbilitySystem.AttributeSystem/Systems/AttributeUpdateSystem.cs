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
        private EntityQuery m_Attributes;

        protected override void OnCreate()
        {
            m_Attributes = GetEntityQuery(typeof(TComponentData));
        }

        protected override void OnUpdate()
        {
            var sumAttributes = new AttributeUpdateJob()
            {
                AttributeDataHandle = GetComponentTypeHandle<TComponentData>(false),
                AttributeModifierDataHandle = GetComponentTypeHandle<TAttributeModifier>(true),
                Executable = default
            };
            this.Dependency = sumAttributes.ScheduleParallel(m_Attributes, this.Dependency);
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
