using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

/// <summary>
///    AbilitySystem is used to manage the entire ability system.
///    It is responsible for:
///        1. Checking to make sure unit has resource to cast
///        2. Checking to make sure ability is off cooldown
///        3. Checking to make sure unit is able to cast ability
///        4. Spawning meshes/running logic/animating required to cast ability
///            (and applying game effects such as costs)
/// </summary>
public abstract class AbilitySystem<TAbility, TCooldown> : JobComponentSystem
where TAbility : struct, IComponentData, IAbilityBehaviour
where TCooldown : struct, ICooldownJob, IJobForEachWithEntity<AbilityCooldownComponent, AbilitySourceTarget>, ICooldownSystemComponentDefinition {

    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // cooldownQuery = GetEntityQuery(cooldownQueryDesc);
        base.OnCreate();
    }

    public struct ActivateAbilityJob : IJobForEachWithEntity<TAbility, AbilityStateComponent, AbilitySourceTarget> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;

        [ReadOnly] public NativeArray<CooldownTimeCaster> CooldownArray;

        public float WorldTime;

        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability, ref AbilityStateComponent abilityState, [ReadOnly] ref AbilitySourceTarget sourceTarget) {
            if (abilityState.State != EAbilityState.TryActivate) return;
            var sourceAttrs = attributesComponent[sourceTarget.Source];
            if (attributesComponent.Exists(sourceTarget.Source)) {
                // Check to make sure we have available resources
                EntityCommandBuffer.AddComponent<AbilityCooldownComponent>(index, entity, new AbilityCooldownComponent()
                {
                    Duration = 0,
                    TimeRemaining = 0
                });

                if (!ResourceAvailable(index, entity, sourceTarget.Source, sourceAttrs, ability)) {
                    AbilityUnsuccessful(index, ability, ref abilityState);
                    return;
                }
                // Check if ability is on cooldown for player
                if (!AbilityNotOnCooldown(index, entity, sourceTarget.Source)) {
                    AbilityUnsuccessful(index, ability, ref abilityState);
                    return;
                }

                ability.ApplyAbilityCosts(index, EntityCommandBuffer, sourceTarget.Source, sourceTarget.Target, sourceAttrs);
                ability.ApplyCooldownEffect(index, EntityCommandBuffer, sourceTarget.Source, WorldTime);

            }
            AbilitySuccessful(index, ability, ref abilityState);
        }

        private bool ResourceAvailable(int index, Entity entity, Entity source, AttributesComponent sourceAttrs, TAbility ability) {
            var resourcesAvailable = ability.CheckResourceAvailable(ref source, ref sourceAttrs);
            if (!resourcesAvailable) {
                return false;
            }
            return true;
        }

        private bool AbilityNotOnCooldown(int index, Entity entity, Entity source) {
            // Find entity in CooldownArray
            for (var i = 0; i < CooldownArray.Length; i++) {
                if (CooldownArray[i].Caster.Equals(source)
                    && CooldownArray[i].TimeRemaining > 0f) {
                    return false;
                }
            }
            return true;
        }

        private void AbilitySuccessful(int index, TAbility ability, ref AbilityStateComponent abilityState) {
            abilityState.State = EAbilityState.Activate;
        }

        private void AbilityUnsuccessful(int index, TAbility ability, ref AbilityStateComponent abilityState) {
            abilityState.State = EAbilityState.Failed;
        }
    }

    [BurstCompile]
    public struct DeallocateNativeArraysJob : IJob {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<CooldownTimeCaster> CooldownArray;
        public void Execute() {
        }
    }

    [BurstCompile]
    public struct EndAbilityJob : IJobForEachWithEntity<TAbility, AbilityCooldownComponent, AbilityStateComponent> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability, ref AbilityCooldownComponent cooldown, ref AbilityStateComponent abilityState) {
            if (abilityState.State == EAbilityState.Failed) {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }
            if (abilityState.State != EAbilityState.Completed) return;
            // EntityCommandBuffer.RemoveComponent<CastingAbilityTagComponent>(index, _.Source);
            if (cooldown.TimeRemaining < 0) {
                EntityCommandBuffer.DestroyEntity(index, entity);
            }
        }
    }

    [BurstCompile]
    public struct SetupAbilityDurationWorldTime : IJobForEach<GameplayEffectDurationComponent> {
        public float WorldTime;
        public void Execute(ref GameplayEffectDurationComponent gameplayEffectDuration) {
            if (gameplayEffectDuration.WorldStartTime <= 0) gameplayEffectDuration.WorldStartTime = WorldTime;
        }
    }

    public EntityQuery cooldownQuery;

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        // var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        // // Initialise cooldown data
        // var cooldownJob = new TCooldown();
        // var cooldownQuery = GetEntityQuery(cooldownJob.CooldownQueryDesc);
        // var cooldownTimes = cooldownQuery.ToComponentDataArray<GameplayEffectDurationComponent>(Allocator.TempJob);
        // var casters = cooldownQuery.ToComponentDataArray<CooldownEffectComponent>(Allocator.TempJob);
        // NativeArray<CooldownTimeCaster> cooldownArray = new NativeArray<CooldownTimeCaster>(cooldownTimes.Length, Allocator.TempJob);
        // for (var i = 0; i < cooldownTimes.Length; i++) {
        //     var cooldownItem = new CooldownTimeCaster
        //     {
        //         Caster = casters[i].Caster,
        //         TimeRemaining = cooldownTimes[i].TimeRemaining,
        //         Duration = cooldownTimes[i].Duration
        //     };
        //     cooldownArray[i] = cooldownItem;
        // }
        // cooldownTimes.Dispose();
        // casters.Dispose();
        // cooldownJob.CooldownArray = cooldownArray;

        // inputDeps = new SetupAbilityDurationWorldTime()
        // {
        //     WorldTime = Time.time
        // }.Schedule(this, inputDeps);

        // // Ability activation job
        // inputDeps = new ActivateAbilityJob()
        // {
        //     attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
        //     EntityCommandBuffer = commandBuffer,
        //     WorldTime = Time.time,
        //     CooldownArray = cooldownArray
        // }.Schedule(this, inputDeps);

        // inputDeps = cooldownJob.Schedule(this, inputDeps);

        // // Ability end job
        // inputDeps = new EndAbilityJob()
        // {
        //     EntityCommandBuffer = commandBuffer
        // }.Schedule(this, inputDeps);

        // inputDeps = new DeallocateNativeArraysJob()
        // {
        //     CooldownArray = cooldownArray
        // }.Schedule(inputDeps);

        // // Cooldown job
        // m_EntityCommandBufferSystem.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }

}


public abstract class AbilityActivationSystem<T1> : ComponentSystem
where T1 : struct, IComponentData {

}
