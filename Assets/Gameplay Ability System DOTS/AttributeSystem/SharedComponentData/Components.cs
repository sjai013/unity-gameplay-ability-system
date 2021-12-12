using Unity.Entities;

namespace GameplayAbilitySystem.SCD
{
    public struct AttributeMasterEntity : ISharedComponentData
    {
        public Entity value;
    }

    public struct HealthAttributeModifier : IComponentData
    {
        public EAttributeModifiers Modifier;
        public float Value;
    }

    public struct HealthBaseValue : IComponentData
    {
        public float Value;
    }

    public struct HealthCurrentValue : IComponentData
    {
        public float Value;
    }
}
