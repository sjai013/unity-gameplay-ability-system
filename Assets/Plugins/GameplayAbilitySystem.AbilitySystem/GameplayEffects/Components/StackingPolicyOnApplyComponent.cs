using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components
{
    public struct StackingPolicyOnApplyComponent : IComponentData, IStackableGameplayEffect
    {
        public EStackingAggregatePolicy AggregatePolicy;
        public EStackingDurationRefreshPolicy DurationRefreshPolicy;
        public EStackingPeriodResetPolicy PeriodResetPolicy;
        public int MaxStacks;
        public int CurrentStacks;

        public void ApplyStackingPolicy()
        {
            throw new System.NotImplementedException();
        }

        public struct StackingPolicyResult
        {
            public int NumberOfStacks;

        }
    }
}