using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class GameplayEffectRemoveExpiredSystem : JobComponentSystem {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    struct Job : IJobForEachWithEntity<GameplayEffectExpired> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref GameplayEffectExpired expiredComponent) {
            EntityCommandBuffer.DestroyEntity(index, entity);
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        var job = new Job() {
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };

        var jobHandle = job.Schedule(this, inputDependencies);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}
