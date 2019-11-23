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

    public List<int> AbilityIdentifiers;

    void Start() {
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityOwnerEntity = AbilityCharacter.AbilityOwnerEntity;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityButtons = AbilityButtons;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityIdentifiers = AbilityIdentifiers;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityIconMaps = AbilityIconMaps;

    }


}

public class AbilityHotbarUpdateSystem : ComponentSystem {
    public Entity AbilityOwnerEntity;

    [SerializeField]
    public List<AbilityHotbarButton> AbilityButtons;

    public List<int> AbilityIdentifiers;


    public List<AbilityIconMap> AbilityIconMaps;


    // Dict of ability types, so we can map it to the appropriate ability button.
    // We have no way of identifying abilities right now from the components, so we'll
    // need to assign an ID type of component along with the AbilityTagComponent

    protected override void OnCreate() {


    }


    protected override void OnUpdate() {
        for (int i = 0; i < AbilityButtons.Count; i++) {
            // reset all cooldowns
            AbilityButtons[i].SetCooldownRemainingPercent(1);
            var abilityIconMap = AbilityIconMaps.FirstOrDefault(x => x.AbilityIdentifier == AbilityIdentifiers[i]);
            if (abilityIconMap != null) {
                // Set icon to what is defined in Ability Icon Maps, if different
                AbilityButtons[i].ImageIcon.sprite = abilityIconMap.Sprite;
                AbilityButtons[i].ImageIcon.color = abilityIconMap.SpriteColor;
            } else {
                AbilityButtons[i].ImageIcon.sprite = null;
                AbilityButtons[i].ImageIcon.color = new Color32(0, 0, 0, 0);
            }
        }

        Entities.ForEach<AbilityOwnerComponent, AbilityCooldownComponent, AbilityStateComponent, AbilityIdentifierComponent>((Entity entity, ref AbilityOwnerComponent abilityOwner, ref AbilityCooldownComponent abilityCooldown, ref AbilityStateComponent state, ref AbilityIdentifierComponent identifier) => {
            // Only do this for the appropriate actor
            if (abilityOwner.Value == AbilityOwnerEntity) {
                // Check our list to see if this ability is defined
                var id = identifier.Value;
                var abilityIdentifierIndex = AbilityIdentifiers.FindIndex(x => x == id);
                if (abilityIdentifierIndex >= 0) {
                    UpdateButton(abilityIdentifierIndex, abilityCooldown.Value.NominalDuration, abilityCooldown.Value.RemainingTime, true);
                }
            }

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
