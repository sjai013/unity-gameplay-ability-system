using Unity.Jobs;

namespace GameplayAbilitySystem.AttributeSystem.DOTS.Components
{
    public interface IAttributeModifiers
    {
        float Add { get; set; }
        float Multiply { get; set; }
        float Overwrite { get; set; }
    }

    public interface IAttributeCurrentValue
    {
        float Value { get; set; }
    }
    public interface IAttributeBaseValue
    {
        float Value { get; set; }
    }

    public interface IAttributeRegisterer
    {
        void RegisterAttribute(GameplayAbilitySystem.AttributeSystem.DOTS.Components.AttributeSystem attributeSystem);
        JobHandle ScheduleJob(GameplayAbilitySystem.AttributeSystem.DOTS.Components.AttributeSystem attributeSystem);
    }
}