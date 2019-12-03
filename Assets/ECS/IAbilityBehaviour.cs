using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
/// <summary>
/// Provides collection of functionality all abilities need to have
/// </summary>
public interface _IAbilityBehaviour {

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
    /// Application of gameplay effects associated with ability (use in Job)
    /// </summary>
    /// <param name="index"></param>
    /// <param name="Ecb"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime);

    /// <summary>
    /// Application of gameplay effects associated with ability (use outside Job)
    /// </summary>
    /// <param name="entityManager"></param>
    /// <param name="Source"></param>
    /// <param name="Target"></param>
    /// <param name="attributesComponent"></param>
    /// <param name="WorldTime"></param>
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

    IAbilityJobs AbilityJobs { get; }

    IEntityQueryDescContainer EntityQueries { get; }

}

public interface IEntityQueryDescContainer {
    EntityQueryDesc BeginAbilityCastJobQueryDesc { get; }
    EntityQueryDesc UpdateCooldownsJobQueryDesc { get; }
    EntityQueryDesc CheckAbilityAvailableJobQueryDesc_UpdateAvailability { get; }
    EntityQueryDesc CheckAbilityAvailableJobQueryDesc_CheckResources { get; }
    EntityQueryDesc CheckAbilityGrantedJobQueryDesc { get; }

}