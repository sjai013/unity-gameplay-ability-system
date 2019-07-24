using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class GameplayEffectSystem : JobComponentSystem {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [ExcludeComponent(typeof(GameplayEffectExpired))]
    struct Job : IJobForEachWithEntity<GameplayEffectDurationComponent> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, ref GameplayEffectDurationComponent durationComponent) {
            var timeRemaining = durationComponent.Duration - (Time.realtimeSinceStartup - durationComponent.WorldStartTime);
            if (timeRemaining <= 0) {
                EntityCommandBuffer.AddComponent(index, entity, new GameplayEffectExpired());
            }
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        var job = new Job() {
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, inputDependencies);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}