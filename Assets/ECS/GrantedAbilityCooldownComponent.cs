using Unity.Entities;

public struct GrantedAbilityCooldownComponent : IComponentData {
    public float TimeRemaining;
    public float Duration;
}
