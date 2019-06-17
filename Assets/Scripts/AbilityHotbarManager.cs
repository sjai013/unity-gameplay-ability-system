using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHotbarManager : MonoBehaviour
{
    public AbilityCharacter AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    public List<AbilityIconMap> AbilityIconMaps;

    void Awake()
    {
        for (int i = 0; i < AbilityCharacter.Abilities.Count; i++)
        {
            if (AbilityButtons.Count > i)
            {
                var abilityGraphic = AbilityIconMaps.FirstOrDefault(x => x.Ability == AbilityCharacter.Abilities[i].Ability);

                if (abilityGraphic != null)
                {
                    AbilityButtons[i].ImageIcon.sprite = abilityGraphic.Sprite;
                    AbilityButtons[i].ImageIcon.color = abilityGraphic.SpriteColor;
                }
            }
        }

    }

    void Update()
    {
        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            var button = AbilityButtons[i];
            (var cooldownElapsed, var cooldownTotal) = AbilityCharacter.GetCooldownOfAbility(i);
            var remainingPercent = 0f;
            if (cooldownTotal != 0)
            {
                remainingPercent = 1 - cooldownElapsed / cooldownTotal;
            }
            button.SetCooldownRemainingPercent(1 - remainingPercent);
        }

    }

}
