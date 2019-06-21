using System;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class StackingPolicy {
        public EStackingType StackingType;
        public int StackLimit;
        public EStackRefreshPolicy StackDurationRefreshPolicy;
        public EStackRefreshPolicy StackPeriodResetPolicy;
        public EStackExpirationPolicy StackExpirationPolicy;
    }

    public enum EStackingType {
        None, AggregatedBySource, AggregatedByTarget
    }

    public enum EStackRefreshPolicy {
        RefreshOnSuccessfulApplication, NeverRefresh
    }

    public enum EStackExpirationPolicy {
        ClearEntireStack, RemoveSingleStackAndRefreshDuration, RefreshDuration
    }
}

