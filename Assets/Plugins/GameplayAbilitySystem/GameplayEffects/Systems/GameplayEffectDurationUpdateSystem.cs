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
}