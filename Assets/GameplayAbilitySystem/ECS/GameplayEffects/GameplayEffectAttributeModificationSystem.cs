using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public abstract class GameplayEffectAttributeModificationSystem<PermanentAttributeModificationJob, TemporaryAttributeModificationJob> : JobComponentSystem
    where PermanentAttributeModificationJob : struct, AttributeModifierJob<PermanentAttributeModification>
    where TemporaryAttributeModificationJob : struct, AttributeModifierJob<TemporaryAttributeModification> {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var AttrComponents = GetComponentDataFromEntity<AttributesComponent>(false);
        var job1 = new PermanentAttributeModificationJob() {
            Ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            AttrComponents = AttrComponents,
            AttributeModifier = GetComponentDataFromEntity<PermanentAttributeModification>(true),
        };

        var job2 = new TemporaryAttributeModificationJob() {
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

public interface AttributeModifierJob<T> : IJobForEachWithEntity<AttributeModificationComponent>
    where T : struct, IComponentData {
    EntityCommandBuffer.Concurrent Ecb { get; set; }
    ComponentDataFromEntity<AttributesComponent> AttrComponents { get; set; }
    ComponentDataFromEntity<T> AttributeModifier { get; set; }
}


public struct TemporaryAttributeModification : IComponentData { }
public struct PermanentAttributeModification : IComponentData { }


