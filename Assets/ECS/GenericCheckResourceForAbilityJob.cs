using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[RequireComponentTag(typeof(AbilityComponent))]
public struct GenericCheckResourceForAbilityJob<T1> : IJobForEach<AbilitySourceTargetComponent, _AbilityStateComponent, T1>
where T1 : struct, IComponentData, IAbilityBehaviour {
    [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
    public void Execute([ReadOnly] ref AbilitySourceTargetComponent abilitySourceTarget, ref _AbilityStateComponent state, [ReadOnly] ref T1 ability) {
        if (state.State != EAbilityState.CheckResource) return;
        var resourceAvailable = false;
        var sourceAttrs = attributesComponent[abilitySourceTarget.Source];
        resourceAvailable = ability.CheckResourceAvailable(ref abilitySourceTarget.Source, ref sourceAttrs);
        if (resourceAvailable) {
            state.State = EAbilityState.PreActivate;
        } else {
            state.State = EAbilityState.Failed;
        }
    }
}
