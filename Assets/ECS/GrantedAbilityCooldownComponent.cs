using Unity.Entities;

public struct _GrantedAbilityCooldownComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
}

public struct CooldownForCasterComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
    public Entity Caster;
}
