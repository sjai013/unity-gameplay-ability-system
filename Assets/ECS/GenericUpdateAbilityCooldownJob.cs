using Unity.Collections;
using Unity.Entities;

public struct GenericUpdateAbilityCooldownJob<T1> : IJobForEach<GrantedAbilityComponent, _GrantedAbilityCooldownComponent, T1>
where T1 : struct, IComponentData, _IAbilityBehaviour {
    [ReadOnly] public NativeHashMap<Entity, _GrantedAbilityCooldownComponent> cooldownsRemainingForAbility;
    public void Execute([ReadOnly] ref GrantedAbilityComponent grantedAbility, ref _GrantedAbilityCooldownComponent cooldown, [ReadOnly] ref T1 ability) {
        cooldownsRemainingForAbility.TryGetValue(grantedAbility.GrantedTo, out var duration);
        cooldown.Duration = duration.Duration;
        cooldown.TimeRemaining = duration.TimeRemaining;
    }
}
