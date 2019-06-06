using System;
using GameplayAbilitySystem.Interfaces;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

namespace GameplayAbilitySystem.Events
{
    [Serializable]
    public class GenericAbilityEvent : UnityEvent<IGameplayAbility>
    {

    }

    [Serializable]
    public class GameplayEvent: UnityEvent<GameplayTag, GameplayEventData> {

    }

    [Serializable]
    public struct GameplayEventData
    {
        public GameplayTag EventTag;
        public AbilitySystemComponent Instigator;
        public AbilitySystemComponent Target;
        public object OptionalObject;
        // Gameplay Effect handle?
        public List<GameplayTag> InstigatorTags;
        public List<GameplayTag> TargetTags;
        public float EventMagnitude;
        // Target Data?
        
        
    }

}