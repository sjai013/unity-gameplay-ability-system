using GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Systems
{
    public class GameplayEffectTurnTickSystem : SystemBase
    {
        public bool m_TickThisFrame { get; private set; } = false;
        public void ScheduleTick()
        {
            m_TickThisFrame = true;
        }

        EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
        NativeQueue<Entity> ScheduledTicks;
        protected override void OnCreate()
        {
            m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            ScheduledTicks = new NativeQueue<Entity>(Allocator.Persistent);
            var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(TimeDurationComponent));
            SetComponent(entity, TimeDurationComponent.New(1f, 20f));
        }
        protected override void OnUpdate()
        {
            if (!m_TickThisFrame) return;

            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
            ScheduledTicks.Clear();
            var tickEvents = ScheduledTicks.AsParallelWriter();

            Entities
                .ForEach((int entityInQueryIndex, Entity entity, ref TurnDurationComponent durationComponent, ref DurationStateComponent state) =>
                {
                    if (durationComponent.Tick())
                    {
                        tickEvents.Enqueue(entity);
                        state.MarkTick();
                    }
                    else
                    {
                        state.State &= ~(EDurationState.TICKED_THIS_FRAME);
                    }

                    if (durationComponent.IsExpired())
                    {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        state.MarkExpired();
                    }

                })
                .WithBurst()
                .ScheduleParallel();
        }
    }
}