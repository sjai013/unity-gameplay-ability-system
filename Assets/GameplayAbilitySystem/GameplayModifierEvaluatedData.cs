using GameplayAbilitySystem.Interfaces;
using GameplayAbilitySystem.Enums;

namespace GameplayAbilitySystem.GameplayEffects {
    public struct GameplayModifierEvaluatedData {
        public IAttribute Attribute;
        public EModifierOperationType ModOperation;
        public float Magnitude;

    }

}

