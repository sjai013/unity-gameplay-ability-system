using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components
{
    public struct StackExpirationPolicyComponent : IComponentData
    {
        public EStackExpirationPolicy StackExpirationPolicy;
    }
}