using System;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Enums;
using GameplayAbilitySystem.Events;

namespace GameplayAbilitySystem.Statics
{
    public class AbilitySystemStatics
    {
        public static void SendGameplayEventToComponent(AbilitySystemComponent TargetAbilitySystem, GameplayTag EventTag, GameplayEventData Payload)
        {
            TargetAbilitySystem.HandleGameplayEvent(EventTag, Payload);
        }

        public static float CalculateModifiedBaseAttribute(float BaseValue, EModifierOperationType ModifierOperation, float EvaluatedMagnitude)
        {
            switch (ModifierOperation)
            {
                case EModifierOperationType.Add:
                    BaseValue += EvaluatedMagnitude;
                    break;
                case EModifierOperationType.Divide:
                    if (Math.Abs(EvaluatedMagnitude) > 0.01) BaseValue /= EvaluatedMagnitude;
                    break;
                case EModifierOperationType.Multiply:
                    BaseValue *= EvaluatedMagnitude;
                    break;
                case EModifierOperationType.Override:
                    BaseValue = EvaluatedMagnitude;
                    break;
            }

            return BaseValue;
        }
    }
}
