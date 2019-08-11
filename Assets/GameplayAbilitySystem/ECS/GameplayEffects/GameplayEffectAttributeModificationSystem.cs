using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(ResetAttributesDeltaSystem))]
[UpdateBefore(typeof(ApplyAttributesDeltaSystem))]
[UpdateBefore(typeof(RemovePermanentAttributeModificationTag))]

public abstract class GameplayEffectAttributeModificationSystem<T> : JobComponentSystem
    where T : struct, IComponentData, AttributeModifier {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [BurstCompile]
    [RequireComponentTag(typeof(PermanentAttributeModification))]
    public struct P_Attribute_ModifierJob : IJobForEach<AttributeModificationComponent, T> {
        public EntityCommandBuffer.Concurrent Ecb { get; set; }
        [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
        [ReadOnly] private ComponentDataFromEntity<PermanentAttributeModification> _attributeModifier;
        public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
        public ComponentDataFromEntity<PermanentAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

        public void Execute(ref AttributeModificationComponent attrMod, [ReadOnly] ref T _) {
            if (_attrComponents.Exists(attrMod.Target)) {
                var attrs = _attrComponents[attrMod.Target];
                _.PermanentAttributeModification(ref attrMod, ref attrs);
                _attrComponents[attrMod.Target] = attrs;
            }
        }
    }

    [BurstCompile]
    [RequireComponentTag(typeof(TemporaryAttributeModification))]
    public struct T_AttributeModifier_ModifierJob : IJobForEach<AttributeModificationComponent, T> {
        public EntityCommandBuffer.Concurrent Ecb { get; set; }
        [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
        [ReadOnly] private ComponentDataFromEntity<TemporaryAttributeModification> _attributeModifier;
        public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
        public ComponentDataFromEntity<TemporaryAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

        public void Execute(ref AttributeModificationComponent attrMod, [ReadOnly] ref T _) {
            if (_attrComponents.Exists(attrMod.Target)) {
                var attrs = _attrComponents[attrMod.Target];
                _.TemporaryAttributeModification(ref attrMod, ref attrs);
                _attrComponents[attrMod.Target] = attrs;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var AttrComponents = GetComponentDataFromEntity<AttributesComponent>(false);
        var job1 = new P_Attribute_ModifierJob()
        {
            Ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            AttrComponents = AttrComponents,
            AttributeModifier = GetComponentDataFromEntity<PermanentAttributeModification>(true),
        };

        var job2 = new T_AttributeModifier_ModifierJob()
        {
            Ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            AttrComponents = AttrComponents,
            AttributeModifier = GetComponentDataFromEntity<TemporaryAttributeModification>(true),
        };

        var jobHandle1 = job1.Schedule(this, inputDeps);
        var jobHandle2 = job2.Schedule(this, jobHandle1);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle2);
        return jobHandle2;

    }
}

public interface AttributeModifierJob<T1, T2> : IJobForEach<AttributeModificationComponent, T2>
    where T1 : struct, IComponentData
    where T2 : struct, IComponentData, AttributeModifier {
    EntityCommandBuffer.Concurrent Ecb { get; set; }
    ComponentDataFromEntity<AttributesComponent> AttrComponents { get; set; }
    ComponentDataFromEntity<T1> AttributeModifier { get; set; }
}


public struct TemporaryAttributeModification : IComponentData { }
public struct PermanentAttributeModification : IComponentData { }