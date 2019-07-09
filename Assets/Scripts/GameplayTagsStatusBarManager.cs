using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.GameplayEffects;
using GameplayAbilitySystem;

public class GameplayTagsStatusBarManager : MonoBehaviour {
    public AbilityCharacter AbilityCharacter;
    public List<GameplayTagStatusBarButton> GameplayTagIndicator;
    public List<GameplayTagIconMap> GameplayTagIcons;

    private Dictionary<GameplayTag, GameplayTagIconMap> availableTagsToShow;

    public GameplayTagsStatusBarManager() {
    }

    void Awake() {
        this.availableTagsToShow = GameplayTagIcons.ToDictionary(x => x.Tag);
    }

    List<(GameplayTag Tag, ActiveGameplayEffectData EffectData, int stacks)> GetTagsToShow() {
        var activeTags = AbilityCharacter.SelfAbilitySystem.ActiveTagsByActiveGameplayEffect;;
        var effectsToShow = activeTags
                            .Where(x => availableTagsToShow
                                                    .ContainsKey(x.Tag))
                                                    .OrderBy(x => x.GrantingEffect.StartWorldTime)
                                                    .Select(x => (x.Tag, x.GrantingEffect, 1))
                            .ToList();
        return effectsToShow;
    }

    List<(GameplayTag Tag, ActiveGameplayEffectData EffectData, int stacks)> GetStackedGameplayTagsToShow() {
        var effectsToShow = GetTagsToShow()
                            .GroupBy(x => x.Tag)
                            .Select(x => (x.Last().Tag, x.Last().EffectData, x.Count()))
                            .ToList();

        return effectsToShow;
    }

    void Update() {
        // var stackedEffectsToShow = GetEffectsToShow();
        var stackedTagsToShow = GetStackedGameplayTagsToShow();
        var tagIndex = 0;
        for (int i = 0; i < stackedTagsToShow.Count; i++) {
            var tagToShow = stackedTagsToShow[i];

            var stacks = stackedTagsToShow[i].stacks;
            // No more space to show buffs - just ignore the rest until space opens up
            if (GameplayTagIndicator.Count < tagIndex) return;

            availableTagsToShow.TryGetValue(tagToShow.Tag, out var iconMap);
            if (iconMap == null) continue;

            var cooldownElapsed = tagToShow.EffectData.CooldownTimeElapsed;
            var cooldownTotal = tagToShow.EffectData.CooldownTimeTotal;

            var remainingPercent = 0f;
            if (cooldownTotal != 0) {
                remainingPercent = 1 - cooldownElapsed / cooldownTotal;
            }

            GameplayTagIndicator[tagIndex].SetCooldownRemainingPercent(1 - remainingPercent);
            GameplayTagIndicator[tagIndex].ImageIcon.sprite = iconMap.Sprite;
            GameplayTagIndicator[tagIndex].ImageIcon.color = iconMap.SpriteColor;
            GameplayTagIndicator[tagIndex].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(true);
            GameplayTagIndicator[tagIndex].SetStacks(stacks);

            ++tagIndex;
        }

        // These are all empty - reset them
        for (int i = tagIndex; i < GameplayTagIndicator.Count; i++) {
            GameplayTagIndicator[i].ImageIcon.sprite = null;
            GameplayTagIndicator[i].ImageIcon.color = new Color(0, 0, 0, 0);
            GameplayTagIndicator[i].SetCooldownRemainingPercent(0);
            GameplayTagIndicator[i].GetComponentInChildren<RectTransform>(true).gameObject.SetActive(false);
            GameplayTagIndicator[i].SetStacks(0);
        }

    }
}
