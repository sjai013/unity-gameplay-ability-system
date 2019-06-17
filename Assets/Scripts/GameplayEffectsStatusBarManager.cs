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

    List<(ActiveGameplayEffectData EffectData, int stacks)> GetEffectsToShow()
    {
        var activeEffectData = AbilityCharacter.SelfAbilitySystem.ActiveGameplayEffectsContainer.ActiveEffectAttributeAggregator.GetActiveEffects();
        var effectsToShow = activeEffectData
                            .Where(x => availableEffectsForShow
                            .ContainsKey(x.Effect))
                            .OrderBy(x => x.StartWorldTime)
                            .Select(x => (x, 1))
                            .ToList();
        return effectsToShow;
    }

    List<(ActiveGameplayEffectData EffectData, int stacks)> GetStackedEffectsToShow()
    {
        var effectsToShow = GetEffectsToShow()
                            .Select(x => x.EffectData)
                            .GroupBy(x => x.Effect)
                            .Select(x =>(x.Last(), x.Count()))
                            .ToList();

        return effectsToShow;
    }

    void Update()
    {
        // var stackedEffectsToShow = GetEffectsToShow();
        var stackedEffectsToShow = GetStackedEffectsToShow();
        var effectNum = 0;
        for (int i = 0; i < stackedEffectsToShow.Count; i++)
        {
            var effectToShow = stackedEffectsToShow[i].EffectData;

            var stacks = stackedEffectsToShow[i].stacks;
            // No more space to show buffs - just ignore the rest until space opens up
            if (GameplayEffectIndicator.Count < effectNum) return;

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
            GameplayEffectIndicator[effectNum].SetStacks(stacks);

            ++effectNum;
        }

        // These are all empty - reset them
        for (int i = effectNum; i < GameplayEffectIndicator.Count; i++)
        {
            GameplayEffectIndicator[i].ImageIcon.sprite = null;
            GameplayEffectIndicator[i].ImageIcon.color = new Color(0, 0, 0, 0);
            GameplayEffectIndicator[i].SetCooldownRemainingPercent(0);
            GameplayEffectIndicator[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
            GameplayEffectIndicator[i].SetStacks(0);
        }

    }

}