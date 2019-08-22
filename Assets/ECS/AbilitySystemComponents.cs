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
/// Tag to indicate the entity is currently casting, and therefore not able to cast
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
