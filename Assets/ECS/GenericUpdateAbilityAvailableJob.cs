using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[RequireComponentTag(typeof(AbilityComponent))]
public struct GenericUpdateAbilityAvailableJob<T1> : IJobForEach<_AbilityStateComponent, AbilitySourceTargetComponent, T1>
where T1 : struct, IComponentData, IAbilityBehaviour {
    [ReadOnly] public NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility;
    public void Execute(ref _AbilityStateComponent state, [ReadOnly] ref AbilitySourceTargetComponent sourceTarget, [ReadOnly] ref T1 ability) {
        if (state.State != EAbilityState.CheckCooldown) return;
        cooldownsRemainingForAbility.TryGetValue(sourceTarget.Source, out var cooldownRemaining);
        if (cooldownRemaining.TimeRemaining > 0) {
            state.State = EAbilityState.Failed;
        } else {
            state.State = EAbilityState.CheckResource;
        }
        // if (cooldown.TimeRemaining > 0 && cooldown.CooldownActivated) {
        // } else {
        //     state.State = EAbilityState.CheckResource;
        // }
    }
}
