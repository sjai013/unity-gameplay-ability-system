using Unity.Entities;

namespace GameplayAbilitySystem.AttributeSystem.Components
{
    public struct GameplayEffectContext : IComponentData
    {
        public Entity Target;
        public Entity Source;
    }
}
