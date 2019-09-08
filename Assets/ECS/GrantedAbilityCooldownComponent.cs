using Unity.Entities;

public struct GrantedAbilityCooldownComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
}

public struct CooldownForCasterComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
    public Entity Caster;
}
