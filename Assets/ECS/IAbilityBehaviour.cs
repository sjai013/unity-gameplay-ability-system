using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
/// <summary>
/// Provides collection of functionality all abilities need to have
/// </summary>
public interface IAbilityBehaviour {

    /// <summary>
    /// Application of costs associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime);

    /// <summary>
    /// Application of gameplay effects associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime);
    void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime);

    /// <summary>
    /// Check for resource availability associated with ability
    /// </summary>
    /// <param name="Caster"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes);

    /// <summary>
    /// Application of cooldown effects associated with ability
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Caster"></param>
    /// <param name="WorldTime"></param>
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);
    EAbility AbilityType { get; }

    EGameplayEffect[] CooldownEffects { get; }

    JobHandle BeginAbilityCastJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime);
    JobHandle UpdateCooldownsJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility);
    JobHandle CheckAbilityAvailableJob(JobComponentSystem system, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent, NativeHashMap<Entity, GrantedAbilityCooldownComponent> abilityCooldowns);
    JobHandle CheckAbilityGrantedJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted);
}
