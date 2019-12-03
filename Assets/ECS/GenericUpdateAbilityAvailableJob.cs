using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[RequireComponentTag(typeof(_AbilityComponent))]
public struct _GenericUpdateAbilityAvailableJob<_T1> : IJobForEach<_AbilityStateComponent, _AbilitySourceTargetComponent, _T1>
where _T1 : struct, IComponentData, _IAbilityBehaviour {
    [ReadOnly] public NativeHashMap<Entity, _GrantedAbilityCooldownComponent> cooldownsRemainingForAbility;
    public void Execute(ref _AbilityStateComponent state, [ReadOnly] ref _AbilitySourceTargetComponent sourceTarget, [ReadOnly] ref _T1 ability) {
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
