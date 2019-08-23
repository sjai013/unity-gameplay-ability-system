using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class AbilityHotbarManager : MonoBehaviour {
    public AbilityCharacter AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    public List<AbilityIconMap> AbilityIconMaps;

    void Awake() {
        for (int i = 0; i < AbilityCharacter.Abilities.Count; i++) {
            if (AbilityButtons.Count > i) {
                var abilityGraphic = AbilityIconMaps.FirstOrDefault(x => x.Ability == AbilityCharacter.Abilities[i].Ability);

                if (abilityGraphic != null) {
                    AbilityButtons[i].ImageIcon.sprite = abilityGraphic.Sprite;
                    AbilityButtons[i].ImageIcon.color = abilityGraphic.SpriteColor;
                }
            }
        }

        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityCharacter = AbilityCharacter;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityButtons = AbilityButtons;

    }

}

public class AbilityHotbarUpdateSystem : ComponentSystem {
    public AbilityCharacter AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    protected override void OnUpdate() {
        // reset all cooldowns
        for (int i = 0; i < AbilityButtons.Count; i++) {
            AbilityButtons[i].SetCooldownRemainingPercent(1);
        }
        Entities.ForEach<FireAbility>((Entity entity, ref FireAbility Ability) => {
            if (Ability.Source == AbilityCharacter.SelfAbilitySystem.entity && Ability.CooldownDuration > 0) {
                UpdateButton(0, Ability.CooldownDuration, Ability.CooldownTimeRemaining);
            }
        });
    }

    private void UpdateButton(int index, float cooldownDuration, float cooldownTimeRemaining) {
        var button = AbilityButtons[index];
        var remainingPercent = 0f;
        if (cooldownDuration != 0) {
            remainingPercent = 1 - cooldownTimeRemaining / cooldownDuration;
        }
        button.SetCooldownRemainingPercent(remainingPercent);
    }


}
