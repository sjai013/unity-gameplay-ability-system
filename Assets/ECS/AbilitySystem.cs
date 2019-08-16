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
where TCooldown : struct, IJobForEachWithEntity<CooldownEffectComponent, GameplayEffectDurationComponent>
// where TCost : struct, IComponentData, ICost 
{

    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        // cooldownQuery = GetEntityQuery(cooldownQueryDesc);
        base.OnCreate();
    }
    [RequireComponentTag(typeof(CheckAbilityConstraintsComponent))]
    public struct ActivateAbilityJob : IJobForEachWithEntity<TAbility> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public float WorldTime;

        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability) {
            var source = ability.Source;
            var sourceAttrs = attributesComponent[ability.Source];
            // Check to make sure we have available resources
            if (attributesComponent.Exists(source)) {
                var resourcesAvailable = ability.CheckResourceAvailable(source, sourceAttrs);
                if (!resourcesAvailable) {
                    EndAbility(index, entity);
                    return;
                }
                ability.ApplyAbilityCosts(index, EntityCommandBuffer, ability.Source, ability.Source, sourceAttrs);
                ability.ApplyCooldownEffect(index, EntityCommandBuffer, source, WorldTime);
            }
            EndAbility(index, entity);
        }
        private void EndAbility(int index, Entity entity) {
            EntityCommandBuffer.AddComponent<EndAbilityComponent>(index, entity);
            EntityCommandBuffer.RemoveComponent<CheckAbilityConstraintsComponent>(index, entity);
        }
    }
    public struct ComputeCooldownsForEachAbilityAndPlayer : IJobForEachWithEntity<CooldownEffectComponent> {
        public void Execute(Entity entity, int index, ref CooldownEffectComponent c0) {
            throw new System.NotImplementedException();
        }
    }

    [RequireComponentTag(typeof(EndAbilityComponent))]
    public struct EndAbilityJob : IJobForEachWithEntity<TAbility> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility _) {
            EntityCommandBuffer.RemoveComponent<CastingAbilityTagComponent>(index, _.Source);
            EntityCommandBuffer.DestroyEntity(index, entity);
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        inputDeps = new ActivateAbilityJob()
        {
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = commandBuffer,
            WorldTime = Time.time
        }.Schedule(this, inputDeps);

        inputDeps = new EndAbilityJob()
        {
            EntityCommandBuffer = commandBuffer
        }.Schedule(this, inputDeps);

        // inputDeps = new TCooldown().Schedule(cooldownQuery, inputDeps);

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
}

public abstract class AbilityCooldownSystem<T> : JobComponentSystem
    where T : struct, ICooldownSystemComponentDefinition {
    private EntityQuery cooldownQuery;
    protected override void OnCreate() {
        cooldownQuery = GetEntityQuery(new T().CooldownQueryDesc);
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        inputDeps = new FireAbilityCooldownJob().Schedule(cooldownQuery, inputDeps);
        return inputDeps;
    }
}

public interface ICooldownSystemComponentDefinition {
    EntityQueryDesc CooldownQueryDesc { get; }
}


// This describes the number of buffer elements that should be reserved
// in chunk data for each instance of a buffer. In this case, 8 integers
// will be reserved (32 bytes) along with the size of the buffer header
// (currently 16 bytes on 64-bit targets)
// [InternalBufferCapacity(4)]
// public struct CostGameplayEffectBufferElement : IBufferElementData
// {
//     // // These implicit conversions are optional, but can help reduce typing.
//     // public static implicit operator int(CostGameplayEffectBufferElement e) { return e.Value; }
//     // public static implicit operator CostGameplayEffectBufferElement(int e) { return new CostGameplayEffectBufferElement { Value = e }; }
//     // Actual value each buffer element will store.
//     public ICost Value;
// }