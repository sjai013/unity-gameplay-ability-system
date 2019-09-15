using Unity.Entities;
/// <summary>
/// Tag to indicate this entity is a cooldown component
/// </summary>
public struct CooldownEffectComponent : IComponentData {
    public Entity Caster;
}
