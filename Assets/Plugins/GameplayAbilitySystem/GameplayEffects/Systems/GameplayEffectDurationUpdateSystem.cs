using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects.Systems {

    [UpdateInGroup(typeof(GameplayEffectGroupUpdateBeginSystem))]
    public class GameplayEffectDurationUpdateSystem : JobComponentSystem {
        [BurstCompile]
        struct GameplayEffectDurationUpdateSystemJob : IJobForEach<GameplayEffectDurationComponent> {
            public float deltaTime;
            public void Execute(ref GameplayEffectDurationComponent duration) {
                duration.RemainingTime -= deltaTime;
                duration.RemainingTime = math.max(0, duration.RemainingTime);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new GameplayEffectDurationUpdateSystemJob
            {
                deltaTime = Time.deltaTime
            };

            // Now that the job is set up, schedule it to be run. 
            return job.Schedule(this, inputDependencies);
        }
    }

    [UpdateInGroup(typeof(GameplayEffectGroupUpdateEndSystem))]
    public class GameplayEffectCleanup : JobComponentSystem {

        public BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            // Create test entities
            var archetype = EntityManager.CreateArchetype(
                typeof(GameplayEffectDurationComponent),
                typeof(GameplayEffectAttributeEntity)
            );
        }

        [BurstCompile]
        struct CleanupJob : IJobForEachWithEntity<GameplayEffectDurationComponent, GameplayEffectAttributeEntity> {
            public EntityCommandBuffer.Concurrent Ecb;
            public void Execute(Entity entity, int index, ref GameplayEffectDurationComponent durationComponent, ref GameplayEffectAttributeEntity attributeEntityComponent) {
                var duration = durationComponent.RemainingTime;
                if (duration <= 0f) {
                    Ecb.DestroyEntity(index, attributeEntityComponent);
                    Ecb.DestroyEntity(index, entity);
                }
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new CleanupJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);

            return inputDeps;
        }
    }
}