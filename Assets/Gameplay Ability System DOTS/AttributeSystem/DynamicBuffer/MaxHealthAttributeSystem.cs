using Unity.Entities;
using Unity.Mathematics;
public class MaxHealthAttributeSystem : BaseAttributeSystem
{
    public struct AttributeModifierBufferElement : IBufferElementData, BaseAttributeSystem.IValue, BaseAttributeSystem.IModifier
    {
        public float _value;
        public EAttributeModifiers _modifier;
        public float Value { get { return _value; } set { _value = value; } }
        public EAttributeModifiers Modifier { get { return _modifier; } set { _modifier = value; } }
    }

    public struct BaseValue : IComponentData, BaseAttributeSystem.IValue
    {
        public float _value;
        public float Value { get { return _value; } set { _value = value; } }
    }

    public struct CurrentValue : IComponentData, BaseAttributeSystem.IValue
    {
        public float _value;
        public float Value { get { return _value; } set { _value = value; } }
    }


    protected override void UpdateAttribute()
    {
        Entities.ForEach((int entityInQueryIndex, ref CurrentValue currentValue, in BaseValue baseValue, in DynamicBuffer<AttributeModifierBufferElement> modifierBuffer) =>
        {
            BaseAttributeSystem.Execute<CurrentValue, BaseValue, AttributeModifierBufferElement>(ref currentValue, in baseValue, in modifierBuffer);
        }).ScheduleParallel();
    }

    public override ComponentType[] GetArchetype()
    {
        return new ComponentType[] {
            new ComponentType(typeof(BaseValue)),
            new ComponentType(typeof(CurrentValue)),
            new ComponentType(typeof(AttributeModifierBufferElement)),
        };
    }
}
