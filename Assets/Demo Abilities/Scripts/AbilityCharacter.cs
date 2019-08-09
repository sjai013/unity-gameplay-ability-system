using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Statics;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Events;
using UniRx.Async;
using UnityEngine;
using System;

public class AbilityCharacter : MonoBehaviour
{
    public GameplayAbilitySystem.AbilitySystemComponent SelfAbilitySystem { get; private set; }

    public List<CastingAbilityContainer> Abilities = new List<CastingAbilityContainer>();

    public List<AbilitySystemComponent> TargetPool;

    // Start is called before the first frame update
    void Start()
    {
        SelfAbilitySystem = GetComponent<GameplayAbilitySystem.AbilitySystemComponent>();
    }


    public (float CooldownElapsed, float CooldownTotal) GetCooldownOfAbility(int n)
    {
        if (n >= this.Abilities.Count) return (0f, 0f);
        var ability = this.Abilities[n].Ability;
        return ability.CalculateCooldown(SelfAbilitySystem);
        // foreach (var item in SelfAbilitySystem.ActiveGameplayEffectsContainer.ActiveCooldowns)
        // {
        //     Debug.Log(item.Effect.GameplayEffectPolicy.DurationMagnitude - item.CooldownTimeElapsed);
        // }
    }

    public void CastAbility(int n)
    {
        if (n >= this.Abilities.Count) return;
        if (this.Abilities[n] == null) return;
        if (this.Abilities[n].Ability == null) return;
        if (this.Abilities[n].AbilityTarget == null) return;

        var Ability = this.Abilities[n].Ability;

        var Target = this.Abilities[n].AbilityTarget;

        //Randomise Target
        if (this.TargetPool.Count > 0) {
            var randomIndex = UnityEngine.Random.Range(0, TargetPool.Count);
            Target = TargetPool[randomIndex];
        }

        var eventTag = Ability.Tags.AbilityTags.Added.Count > 0 ? Ability.Tags.AbilityTags.Added[0] : new GameplayTag();
        var gameplayEventData = new GameplayEventData();
        gameplayEventData.EventTag = eventTag;
        gameplayEventData.Target = Target;


        
        // If ability can be activated
        if (SelfAbilitySystem.TryActivateAbility(Ability))
        {
            // Send gameplay event to this player with information on target etc
            AbilitySystemStatics.SendGameplayEventToComponent(SelfAbilitySystem, eventTag, gameplayEventData);
        }
    }

}

[Serializable]
public class CastingAbilityContainer
{
    public GameplayAbility Ability;

    public GameplayAbilitySystem.AbilitySystemComponent AbilityTarget;
}