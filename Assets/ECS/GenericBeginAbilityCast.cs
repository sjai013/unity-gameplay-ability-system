using Unity.Collections;
using Unity.Entities;

[RequireComponentTag(typeof(AbilityComponent))]
public struct GenericBeginAbilityCast<T1> : IJobForEachWithEntity<AbilityStateComponent, AbilitySourceTargetComponent, T1>
where T1 : struct, IComponentData, IAbilityBehaviour {
    public EntityCommandBuffer.Concurrent Ecb;
    public float WorldTime;
    [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
    public void Execute(Entity entity, int index, [ReadOnly] ref AbilityStateComponent abilityStateComponent, [ReadOnly] ref AbilitySourceTargetComponent abilitySourceTarget, [ReadOnly] ref T1 ability) {
        if (abilityStateComponent.State != EAbilityState.PreActivate) return;
        ability.ApplyCooldownEffect(index, Ecb, abilitySourceTarget.Source, WorldTime);
        ability.ApplyAbilityCosts(index, Ecb, abilitySourceTarget.Source, abilitySourceTarget.Target, attributesComponent[abilitySourceTarget.Source]);
        abilityStateComponent.State = EAbilityState.Activate;
    }
}
