using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;

public abstract class BaseAttributeSystem : GameplayAbilitySystem.Systems.AttributeSystem<EAttributeModifiers>
{
    public static void Execute<T1, T2, T3>(ref T1 currentValue, in T2 baseValue, in DynamicBuffer<T3> modifierBuffer)
    where T1 : struct, IComponentData, IValue
    where T2 : struct, IComponentData, IValue
    where T3 : struct, IBufferElementData, IValue, IModifier
    {
        float add = 0;
        float multiply = 1;
        float overwrite = 0;
        bool useOverwrite = false;
        for (var i = 0; i < modifierBuffer.Length; i++)
        {
            switch (modifierBuffer[i].Modifier)
            {
                case EAttributeModifiers.Add:
                    add += modifierBuffer[i].Value;
                    break;
                case EAttributeModifiers.Multiply:
                    multiply *= modifierBuffer[i].Value;
                    break;
                case EAttributeModifiers.Overwrite:
                    overwrite = modifierBuffer[i].Value;
                    useOverwrite = true;
                    break;
            }
        }
        var value = math.select((baseValue.Value + add) * multiply, overwrite, useOverwrite);
        currentValue.Value = value;
    }

    public abstract ComponentType[] GetArchetype();

    public interface IValue
    {
        float Value { get; set; }
    }

    public interface IModifier
    {
        EAttributeModifiers Modifier { get; set; }
    }

}



