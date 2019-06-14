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
    GameplayAbilitySystem.AbilitySystemComponent SelfAbilitySystem;

    public List<CastingAbilityContainer> Abilities = new List<CastingAbilityContainer>();

    // Start is called before the first frame update
    void Start()
    {
        SelfAbilitySystem = GetComponent<GameplayAbilitySystem.AbilitySystemComponent>();
    }



    public void CastAbility1()
    {


    }

    public void CastAbility(int n)
    {
        if (n >= this.Abilities.Count) return;
        if (this.Abilities[n] == null) return;
        if (this.Abilities[n].Ability == null) return;
        if (this.Abilities[n].AbilityTarget == null) return;
    
        var Ability = this.Abilities[n].Ability;
        var Target = this.Abilities[n].AbilityTarget;
        var eventTag = Ability.Tags.AbilityTags.Count > 0 ? Ability.Tags.AbilityTags[0] : new GameplayTag();
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