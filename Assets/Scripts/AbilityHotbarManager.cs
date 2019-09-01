using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class AbilityHotbarManager : MonoBehaviour {
    public AbilityCharacter AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    public List<AbilityIconMap> AbilityIconMaps;

    void Start() {
        for (int i = 0; i < AbilityCharacter.Abilities.Count; i++) {
            if (AbilityButtons.Count > i) {
                var abilityGraphic = AbilityIconMaps.FirstOrDefault(x => x.Ability == AbilityCharacter.Abilities[i].Ability);

                if (abilityGraphic != null) {
                    AbilityButtons[i].ImageIcon.sprite = abilityGraphic.Sprite;
                    AbilityButtons[i].ImageIcon.color = abilityGraphic.SpriteColor;
                }
            }
        }

        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().CharacterEntity = AbilityCharacter.SelfAbilitySystem.entity;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityButtons = AbilityButtons;

    }

}

public class AbilityHotbarUpdateSystem : ComponentSystem {
    public Entity CharacterEntity;
    public List<AbilityHotbarButton> AbilityButtons;
    EAbility[] AbilityMapping;

    protected override void OnCreate() {
        AbilityMapping = new[] {
            EAbility.FireAbility,
            EAbility.FireAbility,
            EAbility.HealAbility
        };
    }


    protected override void OnUpdate() {
        // reset all cooldowns
        for (int i = 0; i < AbilityButtons.Count; i++) {
            AbilityButtons[i].SetCooldownRemainingPercent(1);
        }
        Entities.ForEach<AbilityComponent, AbilityCooldownComponent, AbilitySourceTarget>((Entity entity, ref AbilityComponent Ability, ref AbilityCooldownComponent cooldown, ref AbilitySourceTarget abilitySourceTarget) => {
            // UpdateButton(0, cooldown.Duration, cooldown.TimeRemaining);
            for (var i = 0; i < AbilityMapping.Length; i++) {
                if (Ability.Ability == AbilityMapping[i] && abilitySourceTarget.Source == CharacterEntity) {
                    UpdateButton(i, cooldown.Duration, cooldown.TimeRemaining, cooldown.CooldownActivated);
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
