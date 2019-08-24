using Unity.Collections;
using Unity.Entities;
/// <summary>
/// Marks the ability system to check if the ability can be activated
/// </summary>
struct CheckAbilityConstraintsComponent : IComponentData { }

/// <summary>
/// Marks the ability system to commit the ability and activate it
/// </summary>
struct CommitJobComponent : IComponentData { }

/// <summary>
/// Internal mark to check when the CommitJobComponent is first added
/// </summary>
struct CommitJobSystemComponent : ISystemStateComponentData { }

/// <summary>
/// /// Tag to indicate the entity is currently casting, and therefore not able to cast
/// </summary>
struct CastingAbilityTagComponent : IComponentData { }

/// <summary>
/// Tag to indicate the ability should end
/// </summary>
struct EndAbilityComponent : IComponentData { }

/// <summary>
/// Tag to indicate this entity is a cooldown component
/// </summary>
public struct CooldownEffectComponent : IComponentData {
    public Entity Caster;
}

/// <summary>
/// Provides collection of functionality all game effects need to have
/// </summary>
public interface IGameplayEffect {
    void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent);
}

/// <summary>
/// Provides collection of functionality that cooldowns need to have
/// </summary>
public interface ICooldown {
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);
}

/// <summary>
/// Provides collection of functionality that cost effects need to have
/// </summary>
public interface ICost : IGameplayEffect {
    AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes);
}


public interface ICooldownSystemComponentDefinition {
    EntityQueryDesc CooldownQueryDesc { get; }
}


public struct CooldownTimeCaster {
    public Entity Caster;
    public float TimeRemaining;
    public float Duration;
}

public struct AbilityCooldownComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
}
public interface ICooldownJob {
    NativeArray<CooldownTimeCaster> CooldownArray { get; set; }
}

public struct AbilityStateComponent : IComponentData {
    public EAbilityState State;

    public static implicit operator EAbilityState(AbilityStateComponent AbilityState) {
        return AbilityState;
    }

    public static implicit operator AbilityStateComponent(EAbilityState e) {
        return new AbilityStateComponent
        {
            State = e
        };
    }
}

/// <summary>
/// Provides collection of functionality all abilities need to have
/// </summary>
public interface IAbility {

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

public struct AbilityComponent : IComponentData {
    public EAbility Ability;
    public static implicit operator EAbility(AbilityComponent e) {
        return e;
    }

    public static implicit operator AbilityComponent(EAbility e) {
        return new AbilityComponent
        {
            Ability = e
        };
    }
}

public enum EAbilityState { TryActivate, Activate, Active, Activated, Completed, Failed }

public enum EAbility {
    FireAbility,
    HealAbility

}

public struct AbilitySourceTarget : IComponentData {
    public Entity Source;
    public Entity Target;

}
