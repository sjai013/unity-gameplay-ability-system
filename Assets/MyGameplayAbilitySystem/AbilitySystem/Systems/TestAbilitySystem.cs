using GameplayAbilitySystem.Abilities.Systems;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;
using UnityEngine;

public struct TestAbilityTag : IAbilityTagComponent, IComponentData {
    public GameplayEffectDurationComponent _durationComponent;
    public GameplayEffectDurationComponent DurationComponent { get => _durationComponent; set => _durationComponent = value; }
}

public class TestAbilitySystem : AbilitySystem<TestAbilityTag> {

    private EntityQuery _cooldownEffectsQuery;
    protected override void OnCreate() {
        base.OnCreate();
        EntityQueryDesc _cooldownQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
            Any = new ComponentType[] { ComponentType.ReadOnly<GlobalCooldownGameplayEffectComponent>() }
        };
        _cooldownEffectsQuery = GetEntityQuery(_cooldownQueryDesc);

    }

    protected override EntityQuery CooldownEffectsQuery => _cooldownEffectsQuery;
}