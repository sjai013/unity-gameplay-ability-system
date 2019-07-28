using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class GameplayEffectUpdateDurationSystem : JobComponentSystem {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [ExcludeComponent(typeof(GameplayEffectExpired))]
    struct Job : IJobForEachWithEntity<GameplayEffectDurationComponent> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public float realTimeSinceStartup;
        public void Execute(Entity entity, int index, ref GameplayEffectDurationComponent durationComponent) {
            durationComponent.TimeRemaining = durationComponent.Duration - (realTimeSinceStartup - durationComponent.WorldStartTime);
            if (durationComponent.TimeRemaining <= 0) {
                EntityCommandBuffer.AddComponent(index, entity, new GameplayEffectExpired());
            }
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        var job = new Job() {
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            realTimeSinceStartup = Time.realtimeSinceStartup
        };

        var jobHandle = job.Schedule(this, inputDependencies);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        return jobHandle;
    }
}

