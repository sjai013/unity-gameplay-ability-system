using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[RequireComponentTag(typeof(_AbilityComponent))]
public struct GenericBeginAbilityCast<T1> : IJobForEachWithEntity<_AbilityStateComponent, _AbilitySourceTargetComponent, T1>
where T1 : struct, IComponentData, _IAbilityBehaviour {
    public EntityCommandBuffer.Concurrent Ecb;
    public float WorldTime;
    [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
    public void Execute(Entity entity, int index, [ReadOnly] ref _AbilityStateComponent abilityStateComponent, [ReadOnly] ref _AbilitySourceTargetComponent abilitySourceTarget, [ReadOnly] ref T1 ability) {
        if (abilityStateComponent.State != EAbilityState.PreActivate) return;
        ability.ApplyCooldownEffect(index, Ecb, abilitySourceTarget.Source, WorldTime);
        ability.ApplyAbilityCosts(index, Ecb, abilitySourceTarget.Source, abilitySourceTarget.Target, attributesComponent[abilitySourceTarget.Source], WorldTime);
        abilityStateComponent.State = EAbilityState.Activate;
    }
}
