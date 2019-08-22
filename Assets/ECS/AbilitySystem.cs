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
where TAbility : struct, IComponentData, IAbility
where TCooldown : struct, ICooldownJob, IJobForEachWithEntity<TAbility>, ICooldownSystemComponentDefinition {

    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // cooldownQuery = GetEntityQuery(cooldownQueryDesc);
        base.OnCreate();
    }

    [RequireComponentTag(typeof(TryActivatingAbilityComponent))]
    public struct ActivateAbilityJob : IJobForEachWithEntity<TAbility> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;

        [ReadOnly] public NativeArray<CooldownTimeCaster> CooldownArray;

        public float WorldTime;

        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability) {
            var source = ability.Source;
            var sourceAttrs = attributesComponent[ability.Source];
            if (attributesComponent.Exists(source)) {
                // Check to make sure we have available resources
                if (!ResourceAvailable(index, ref entity, ref source, ref sourceAttrs, ref ability)) {
                    AbilityUnsuccessful(index, entity);
                    return;
                }
                // Check if ability is on cooldown for player
                if (!AbilityNotOnCooldown(index, ref entity, ref source)) {
                    AbilityUnsuccessful(index, entity);
                    return;
                }

                ability.ApplyAbilityCosts(index, EntityCommandBuffer, ability.Source, ability.Source, sourceAttrs);
                ability.ApplyCooldownEffect(index, EntityCommandBuffer, source, WorldTime);
            }
            AbilitySuccessful(index, entity);
        }

        private bool ResourceAvailable(int index, ref Entity entity, ref Entity source, ref AttributesComponent sourceAttrs, ref TAbility ability) {
            var resourcesAvailable = ability.CheckResourceAvailable(source, sourceAttrs);
            if (!resourcesAvailable) {
                return false;
            }
            return true;
        }

        private bool AbilityNotOnCooldown(int index, ref Entity entity, ref Entity source) {
            // Find entity in CooldownArray
            for (var i = 0; i < CooldownArray.Length; i++) {
                if (CooldownArray[i].Caster.Equals(source)
                    && CooldownArray[i].TimeRemaining > 0f) {
                    return false;
                }
            }
            return true;
        }

        private void AbilitySuccessful(int index, Entity entity) {
            EntityCommandBuffer.RemoveComponent<TryActivatingAbilityComponent>(index, entity);
            EntityCommandBuffer.AddComponent<ActivateAbilityComponent>(index, entity);
            // EntityCommandBuffer.AddComponent<AbilityActivatedComponent>(index, entity);
        }
        private void AbilityUnsuccessful(int index, Entity entity) {
            EntityCommandBuffer.RemoveComponent<TryActivatingAbilityComponent>(index, entity);
            EntityCommandBuffer.AddComponent<EndAbilityComponent>(index, entity);
        }
    }

    [BurstCompile]
    public struct DeallocateNativeArraysJob : IJob {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<CooldownTimeCaster> CooldownArray;
        public void Execute() {
        }
    }
    [RequireComponentTag(typeof(EndAbilityComponent))]
    public struct EndAbilityJob : IJobForEachWithEntity<TAbility> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility _) {
            // EntityCommandBuffer.RemoveComponent<CastingAbilityTagComponent>(index, _.Source);
            EntityCommandBuffer.DestroyEntity(index, entity);
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var cooldownJob = new TCooldown();

        // Initialise cooldown data
        var cooldownQuery = GetEntityQuery(cooldownJob.CooldownQueryDesc);
        var cooldownTimes = cooldownQuery.ToComponentDataArray<GameplayEffectDurationComponent>(Allocator.TempJob);
        var casters = cooldownQuery.ToComponentDataArray<CooldownEffectComponent>(Allocator.TempJob);
        NativeArray<CooldownTimeCaster> cooldownArray = new NativeArray<CooldownTimeCaster>(cooldownTimes.Length, Allocator.TempJob);
        for (var i = 0; i < cooldownTimes.Length; i++) {
            var cooldownItem = new CooldownTimeCaster
            {
                Caster = casters[i].Caster,
                TimeRemaining = cooldownTimes[i].TimeRemaining
            };
            cooldownArray[i] = cooldownItem;
        }
        cooldownTimes.Dispose();
        casters.Dispose();
        cooldownJob.CooldownArray = cooldownArray;


        // Ability activation job
        inputDeps = new ActivateAbilityJob()
        {
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = commandBuffer,
            WorldTime = Time.time,
            CooldownArray = cooldownArray
        }.Schedule(this, inputDeps);

        inputDeps = cooldownJob.Schedule(this, inputDeps);

        // Ability end job
        inputDeps = new EndAbilityJob()
        {
            EntityCommandBuffer = commandBuffer
        }.Schedule(this, inputDeps);

        inputDeps = new DeallocateNativeArraysJob()
        {
            CooldownArray = cooldownArray
        }.Schedule(inputDeps);

        // Cooldown job
        m_EntityCommandBufferSystem.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }

}


/// <summary>
/// Provides collection of functionality all abilities need to have
/// </summary>
public interface IAbility {
    Entity Target { get; set; }
    Entity Source { get; set; }
    float CooldownTimeRemaining { get; set; }

    /// <summary>
    /// Application of costs associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);

    /// <summary>
    /// Application of gameplay effects associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);

    /// <summary>
    /// Check for resource availability associated with ability
    /// </summary>
    /// <param name="Caster"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes);

    /// <summary>
    /// Application of cooldown effects associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Caster"></param>
    /// <param name="WorldTime"></param>
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);
    void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent);
}



public abstract class AbilityCooldownSystem<T0, T1> : JobComponentSystem
    where T0 : struct, IComponentData
    where T1 : struct, ICooldownJob, IJobForEachWithEntity<T0>, ICooldownSystemComponentDefinition {
    protected override void OnCreate() {
        //cooldownQuery = GetEntityQuery((new T1()).CooldownQueryDesc);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        //NativeArray<Entity> targetEntityArray = cooldownQuery.ToEntityArray(Allocator.TempJob);

        var job = new T1();

        var cooldownQuery = GetEntityQuery(job.CooldownQueryDesc);
        var cooldownTimes = cooldownQuery.ToComponentDataArray<GameplayEffectDurationComponent>(Allocator.TempJob);
        var casters = cooldownQuery.ToComponentDataArray<CooldownEffectComponent>(Allocator.TempJob);
        NativeArray<CooldownTimeCaster> cooldownArray = new NativeArray<CooldownTimeCaster>(cooldownTimes.Length, Allocator.TempJob);

        for (var i = 0; i < cooldownTimes.Length; i++) {
            cooldownArray[i] = new CooldownTimeCaster
            {
                Caster = casters[i].Caster,
                TimeRemaining = cooldownTimes[i].TimeRemaining
            };
        }

        cooldownTimes.Dispose();
        casters.Dispose();
        job.CooldownArray = cooldownArray;
        inputDeps = job.Schedule(this, inputDeps);

        return inputDeps;
    }
}

public interface ICooldownSystemComponentDefinition {
    EntityQueryDesc CooldownQueryDesc { get; }
}


public struct CooldownTimeCaster {
    public Entity Caster;
    public float TimeRemaining;
}

public interface ICooldownJob {
    NativeArray<CooldownTimeCaster> CooldownArray { get; set; }
}

/// <summary>
/// Tag to check if ability can be activated (resource, cooldown)
/// </summary>
public struct TryActivatingAbilityComponent : IComponentData { }

/// <summary>
/// Tag to activate ability (run using Hybrid ECS - ComponentSystem)
/// </summary>
public struct ActivateAbilityComponent : IComponentData { }
public struct AbilityActiveComponent : IComponentData { }

/// <summary>
/// Tag to specify ability has been activated, and can be cleaned up 
/// TODO: What does it mean to clean up an ability here?  Need to better define this process.
/// </summary>
public struct AbilityActivatedComponent : IComponentData { }

public abstract class AbilityActivationSystem<T1, T2> : ComponentSystem
where T1 : struct, IComponentData, IAbility
where T2 : class {

}