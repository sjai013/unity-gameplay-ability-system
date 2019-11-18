using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Abilities.Components;

public class AbilityHotbarManager : MonoBehaviour {
    public ActorAbilitySystem AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    public List<AbilityIconMap> AbilityIconMaps;

    void Start() {
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityOwnerEntity = AbilityCharacter.AbilityOwnerEntity;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityButtons = AbilityButtons;
    }


}

public class AbilityHotbarUpdateSystem : ComponentSystem {
    public Entity AbilityOwnerEntity;

    [SerializeField]
    public List<AbilityHotbarButton> AbilityButtons = new List<AbilityHotbarButton>();

    private Dictionary<int, int> AbilityIdentifiers;
    EAbility[] AbilityMapping;

    // Dict of ability types, so we can map it to the appropriate ability button.
    // We have no way of identifying abilities right now from the components, so we'll
    // need to assign an ID type of component along with the AbilityTagComponent

    protected override void OnCreate() {
        AbilityMapping = new[] {
            EAbility.FireAbility,
            EAbility.FireAbility,
            EAbility.HealAbility
        };

        AbilityIdentifiers = new Dictionary<int, int>
        {
            {1,0},
            {2,1},
            {3,3},
            {4,4},
            {5,5},
            {6,6},
            {7,7},
            {8,8},
        };

    }


    protected override void OnUpdate() {
        // reset all cooldowns
        for (int i = 0; i < AbilityButtons.Count; i++) {
            AbilityButtons[i].SetCooldownRemainingPercent(1);
        }

        Entities.ForEach<AbilityOwnerComponent, AbilityCooldownComponent, AbilityStateComponent, AbilityIdentifierComponent>((Entity entity, ref AbilityOwnerComponent abilityOwner, ref AbilityCooldownComponent abilityCooldown, ref AbilityStateComponent state, ref AbilityIdentifierComponent identifier) => {
            if (AbilityIdentifiers.ContainsKey(identifier) && abilityOwner.Value == AbilityOwnerEntity) {
                UpdateButton(AbilityIdentifiers[identifier], abilityCooldown.Value.NominalDuration, abilityCooldown.Value.RemainingTime, true);
            }
            // UpdateButton(0, cooldown.Duration, cooldown.TimeRemaining);
            // for (var i = 0; i < AbilityMapping.Length; i++) {
            //     if (Ability.Ability == AbilityMapping[i] && grantedAbility.GrantedTo == AbilityOwnerEntity) {
            //         UpdateButton(i, cooldown.Duration, cooldown.TimeRemaining, cooldown.Duration > 0);
            //     }
            // }
        });
    }
    private void UpdateButton(int index, float cooldownDuration, float cooldownTimeRemaining, bool cooldownActive) {
        var button = AbilityButtons[index];
        var remainingPercent = 1f;
        if (cooldownDuration != 0) {
            remainingPercent = 1 - cooldownTimeRemaining / cooldownDuration;
        }
        button.SetCooldownRemainingPercent(remainingPercent);
    }


}
