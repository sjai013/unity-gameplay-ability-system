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

public class AbilityCharacter : MonoBehaviour
{
    GameplayAbilitySystem.AbilitySystemComponent SelfAbilitySystem;
    public GameplayAbility Ability1;
    public GameplayAbility Ability2;
    public GameplayAbilitySystem.AbilitySystemComponent Target1;
    public GameplayAbilitySystem.AbilitySystemComponent Target2;
    // Start is called before the first frame update
    void Start()
    {
        SelfAbilitySystem = GetComponent<GameplayAbilitySystem.AbilitySystemComponent>();
    }



    public void CastAbility1()
    {
        var eventTag = Ability1.Tags.AbilityTags.Count > 0 ? Ability1.Tags.AbilityTags[0] : new GameplayTag();
        var gameplayEventData = new GameplayEventData();
        gameplayEventData.EventTag = eventTag;
        gameplayEventData.Target = this.Target1;

        // If ability can be activated
        if (SelfAbilitySystem.TryActivateAbility(Ability1))
        {
            // Send gameplay event to this player with information on target etc
            AbilitySystemStatics.SendGameplayEventToComponent(SelfAbilitySystem, eventTag, gameplayEventData);
        }

    }

    public void CastAbility2()
    {
        var eventTag = Ability2.Tags.AbilityTags.Count > 0 ? Ability2.Tags.AbilityTags[0] : new GameplayTag();
        var gameplayEventData = new GameplayEventData();
        gameplayEventData.EventTag = eventTag;
        gameplayEventData.Target = this.Target2;

        // If ability can be activated
        if (SelfAbilitySystem.TryActivateAbility(Ability2))
        {
            // Send gameplay event to this player with information on target etc
            AbilitySystemStatics.SendGameplayEventToComponent(SelfAbilitySystem, eventTag, gameplayEventData);
        }

    }    


}
