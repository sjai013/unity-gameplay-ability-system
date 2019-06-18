using System;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.Events;

namespace GameplayAbilitySystem.Statics {
    public class AbilitySystemStatics {
        public static void SendGameplayEventToComponent(AbilitySystemComponent TargetAbilitySystem, GameplayTag EventTag, GameplayEventData Payload) {
            TargetAbilitySystem.HandleGameplayEvent(EventTag, Payload);
        }
    }
}
