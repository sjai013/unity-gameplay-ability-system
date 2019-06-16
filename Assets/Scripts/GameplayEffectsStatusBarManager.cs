using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.GameplayEffects;

public class GameplayEffectsStatusBarManager : MonoBehaviour
{
    public AbilityCharacter AbilityCharacter;
    public List<GameplayEffectStatusBarButton> GameplayEffectIndicator;
    public List<GameplayEffectIconMap> GameplayEffectIcons;

    private Dictionary<GameplayEffect, GameplayEffectIconMap> availableEffectsForShow;

    void Awake()
    {
        this.availableEffectsForShow = GameplayEffectIcons.ToDictionary(x => x.GameEffect);
    }

    List<ActiveGameplayEffectData> GetEffectsToShow()
    {
        var activeEffectData = AbilityCharacter.SelfAbilitySystem.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.GetActiveEffects();
        var effectsToShow = activeEffectData.Where(x => availableEffectsForShow.ContainsKey(x.Effect)).OrderBy(x => x.StartWorldTime).ToList();
        return effectsToShow;

    }

    void Update()
    {
        var effectsToShow = GetEffectsToShow();

        var effectNum = 0;
        for (int i = 0; i < effectsToShow.Count; i++)
        {
            // No more space to show buffs - just ignore the rest until space opens up
            if (GameplayEffectIndicator.Count < effectNum) return;

            var effectToShow = effectsToShow[i];
            availableEffectsForShow.TryGetValue(effectToShow.Effect, out var iconMap);
            if (iconMap == null) continue;

            var cooldownElapsed = effectToShow.CooldownTimeElapsed;
            var cooldownTotal = effectToShow.CooldownTimeTotal;

            var remainingPercent = 0f;
            if (cooldownTotal != 0)
            {
                remainingPercent = 1 - cooldownElapsed / cooldownTotal;
            }

            GameplayEffectIndicator[effectNum].SetCooldownRemainingPercent(1 - remainingPercent);
            GameplayEffectIndicator[effectNum].ImageIcon.sprite = iconMap.Sprite;
            GameplayEffectIndicator[effectNum].ImageIcon.color = iconMap.SpriteColor;
            GameplayEffectIndicator[effectNum].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(true);

            ++effectNum;
        }

        // These are all empty - reset them
        for (int i = effectNum; i < GameplayEffectIndicator.Count; i++)
        {
            GameplayEffectIndicator[i].ImageIcon.sprite = null;
            GameplayEffectIndicator[i].ImageIcon.color = new Color(0, 0, 0, 0);
            GameplayEffectIndicator[i].SetCooldownRemainingPercent(0);
            GameplayEffectIndicator[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
        }

    }

}