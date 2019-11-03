using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    [UpdateInGroup(typeof(GameplayEffectGroupUpdateEndSystem))]
    public class GameplayEffectCleanupSystem : JobComponentSystem {

        public BeginSimulationEntityCommandBufferSystem m_EntityCommandBuffer;
        private EntityQuery m_Group;
        protected override void OnCreate() {
            m_EntityCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_Group = GetEntityQuery(ComponentType.ReadOnly<GameplayEffectDurationComponent>());
            // Create test entities
            // var archetype = EntityManager.CreateArchetype(
            //     typeof(GameplayEffectDurationComponent),
            //     typeof(GameplayEffectAttributeEntityComponent)
            // );
        }

        [BurstCompile]
        struct CleanupJob : IJobChunk {
            public EntityCommandBuffer.Concurrent Ecb;
            [ReadOnly] public ArchetypeChunkComponentType<GameplayEffectDurationComponent> DurationComponents;
            [ReadOnly] public ArchetypeChunkComponentType<GameplayEffectAttributeEntityComponent> AttributeEntityComponents;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex) {

                var chunkDurations = chunk.GetNativeArray(DurationComponents);
                var hasAttribute = chunk.Has<GameplayEffectAttributeEntityComponent>(AttributeEntityComponents);
                var chunkAttributeEntities = new NativeArray<GameplayEffectAttributeEntityComponent>();
                var chunkEntities = chunk.GetNativeArray(EntityType);
                if (hasAttribute) {
                    chunkAttributeEntities = chunk.GetNativeArray<GameplayEffectAttributeEntityComponent>(AttributeEntityComponents);
                }
                for (var i = 0; i < chunk.Count; i++) {
                    var Entity = chunkEntities[i];
                    var durationComponent = chunkDurations[i];
                    var duration = durationComponent.RemainingTime;
                    if (duration <= 0f) {
                        if (hasAttribute) {
                            Ecb.DestroyEntity(chunkIndex, chunkAttributeEntities[i]);
                        }
                        Ecb.DestroyEntity(chunkIndex, Entity);
                    }
                }
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = new CleanupJob
            {
                Ecb = m_EntityCommandBuffer.CreateCommandBuffer().ToConcurrent(),
                EntityType = GetArchetypeChunkEntityType(),
                DurationComponents = GetArchetypeChunkComponentType<GameplayEffectDurationComponent>(),
                AttributeEntityComponents = GetArchetypeChunkComponentType<GameplayEffectAttributeEntityComponent>()
            }.Schedule(m_Group, inputDeps);

            m_EntityCommandBuffer.AddJobHandleForProducer(inputDeps);

            return inputDeps;
        }
    }
}