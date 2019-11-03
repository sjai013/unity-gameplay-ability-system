using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    [UpdateInGroup(typeof(GameplayEffectGroupUpdateEndSystem))]
    public class GameplayEffectCleanupSystem : JobComponentSystem {

        public BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            // Create test entities
            var archetype = EntityManager.CreateArchetype(
                typeof(GameplayEffectDurationComponent),
                typeof(GameplayEffectAttributeEntityComponent)
            );
        }

        [BurstCompile]
        struct CleanupJob : IJobForEachWithEntity<GameplayEffectDurationComponent, GameplayEffectAttributeEntityComponent> {
            public EntityCommandBuffer.Concurrent Ecb;
            public void Execute(Entity entity, int index, ref GameplayEffectDurationComponent durationComponent, ref GameplayEffectAttributeEntityComponent attributeEntityComponent) {
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