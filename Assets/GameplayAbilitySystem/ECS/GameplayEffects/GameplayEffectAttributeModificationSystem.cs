using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

public abstract class GameplayEffectAttributeModificationSystem<T> : JobComponentSystem where T : struct, AttributeModifierJob {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new T() {
            Ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            attrComponents = GetComponentDataFromEntity<AttributesComponent>(false)
        };

        var jobHandle = job.Schedule(this, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;

    }
}

public interface AttributeModifierJob : IJobForEachWithEntity<AttributeModificationComponent> {
    EntityCommandBuffer.Concurrent Ecb { get; set; }
    ComponentDataFromEntity<AttributesComponent> attrComponents { get; set; }
}
public class GameplayEffectAttributeModificationUndoSystem<T> : JobComponentSystem where T : struct, AttributeModifierUndoJob {
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new T() {
            attrComponents = GetComponentDataFromEntity<AttributesComponent>(false)
        };

        var jobHandle = job.Schedule(this, inputDeps);

        return jobHandle;

    }
}

public interface AttributeModifierUndoJob : IJobForEachWithEntity<AttributeModificationComponent> {
    ComponentDataFromEntity<AttributesComponent> attrComponents { get; set; }
}

