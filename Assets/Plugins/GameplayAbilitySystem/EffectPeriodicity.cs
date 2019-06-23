using System;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class EffectPeriodicity {
        public float Period;
        public bool ExecuteOnApplication;
        public GameplayEffect ApplyGameEffectOnExecute;
    }

}

