using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.AttributeSystem.DOTS.Components
{
    public abstract class AttributeSystem : SystemBase
    {
        EntityQuery m_Query;
        Dictionary<(System.Type, System.Type, System.Type), EntityQuery> m_Querys = new Dictionary<(System.Type, System.Type, System.Type), EntityQuery>();

        protected void RegisterAttribute<T1, T2, T3>()
        where T1 : struct, IComponentData, IAttributeCurrentValue
        where T2 : struct, IComponentData, IAttributeBaseValue
        where T3 : struct, IComponentData, IAttributeModifiers
        {
            var query = GetEntityQuery(
                typeof(T1), ComponentType.ReadOnly<T2>(), ComponentType.ReadOnly<T3>()
            );
            m_Querys.Add((typeof(T1), typeof(T2), typeof(T3)), query);
        }

        protected JobHandle ScheduleJob<T1, T2, T3>()
        where T1 : struct, IComponentData, IAttributeCurrentValue
        where T2 : struct, IComponentData, IAttributeBaseValue
        where T3 : struct, IComponentData, IAttributeModifiers
        {
            var query = m_Querys[(typeof(T1), typeof(T2), typeof(T3))];
            var currentValueType = GetComponentTypeHandle<T1>();
            var baseValueType = GetComponentTypeHandle<T2>();
            var modifierValueType = GetComponentTypeHandle<T3>();

            var job = new AttributeSystemJob<T1, T2, T3>()
            {
                CurrentValueHandle = currentValueType,
                BaseValueHandle = baseValueType,
                ModifiersHandle = modifierValueType
            };

            Dependency = job.ScheduleParallel(query, 1, Dependency);
            return Dependency;
        }

        [BurstCompile]
        public struct AttributeSystemJob<T1, T2, T3> : IJobEntityBatch
        where T1 : struct, IComponentData, IAttributeCurrentValue
        where T2 : struct, IComponentData, IAttributeBaseValue
        where T3 : struct, IComponentData, IAttributeModifiers
        {
            public ComponentTypeHandle<T1> CurrentValueHandle;
            [ReadOnly] public ComponentTypeHandle<T2> BaseValueHandle;
            [ReadOnly] public ComponentTypeHandle<T3> ModifiersHandle;
            public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
            {
                var chunkCurrentValues = batchInChunk.GetNativeArray(CurrentValueHandle);
                var chunkBaseValues = batchInChunk.GetNativeArray(BaseValueHandle);
                var chunkModifierValues = batchInChunk.GetNativeArray(ModifiersHandle);

                for (var i = 0; i < chunkCurrentValues.Length; i++)
                {
                    var chunkCurrentValue = chunkCurrentValues[i];
                    var chunkBaseValue = chunkBaseValues[i];
                    var chunkModifierValue = chunkModifierValues[i];

                    chunkCurrentValue.Value = (chunkBaseValue.Value + chunkModifierValue.Add) * chunkModifierValue.Multiply;
                    chunkCurrentValues[i] = chunkCurrentValue;
                }
            }
        }
    }
}