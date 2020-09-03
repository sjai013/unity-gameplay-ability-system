using System.Runtime.CompilerServices;
using GameplayAbilitySystem.AttributeSystem.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem
{
    public struct AttributeValues : IAttributeData, IComponentData
    {
        public MyPlayerAttributes<uint> BaseValue;
        public MyPlayerAttributes<uint> CurrentValue;
    }

    public struct MyGameplayAttributeModifier : IBufferElementData, IGameplayAttributeModifier<MyAttributeModifierValues>
    {
        public half Value;
        public EMyPlayerAttribute Attribute;
        public EMyAttributeModifierOperator Operator;
        ref MyPlayerAttributes<float> GetAttributeCollection(ref MyAttributeModifierValues attributeModifier)
        {
            switch (Operator)
            {
                case EMyAttributeModifierOperator.Add:
                    return ref attributeModifier.AddValue;
                case EMyAttributeModifierOperator.Multiply:
                    return ref attributeModifier.DivideValue;
                case EMyAttributeModifierOperator.Divide:
                    return ref attributeModifier.MultiplyValue;
                default:
                    return ref attributeModifier.AddValue;
            }
        }

        public void UpdateAttribute(ref MyAttributeModifierValues attributeModifier)
        {

            ref var attributeGroup = ref GetAttributeCollection(ref attributeModifier);

            switch (Attribute)
            {
                case EMyPlayerAttribute.Health:
                    attributeGroup.Health += Value;
                    break;
                case EMyPlayerAttribute.MaxHealth:
                    attributeGroup.MaxHealth += Value;
                    break;
                case EMyPlayerAttribute.Mana:
                    attributeGroup.Mana += Value;
                    break;
                case EMyPlayerAttribute.MaxMana:
                    attributeGroup.MaxMana += Value;
                    break;
                case EMyPlayerAttribute.Speed:
                    attributeGroup.Speed += Value;
                    break;
                default:
                    return;
            }

        }
    }

    public enum EMyPlayerAttribute
    {
        Health, MaxHealth, Mana, MaxMana, Speed
    }

    public enum EMyAttributeModifierOperator
    {
        Add, Multiply, Divide
    }


    public struct MyAttributeModifierValues : IAttributeModifier, IComponentData
    {
        public MyPlayerAttributes<float> AddValue;
        public MyPlayerAttributes<float> MultiplyValue;
        public MyPlayerAttributes<float> DivideValue;
    }

    public struct MyPlayerAttributes<T>
    where T : struct
    {
        public T Health;
        public T MaxHealth;
        public T Mana;
        public T MaxMana;
        public T Speed;
    }


    public struct MyPlayerAttributesJob : IAttributeExecute<AttributeValues, MyAttributeModifierValues>
    {
        public void Execute(NativeArray<AttributeValues> attributeValuesChunk, NativeArray<MyAttributeModifierValues> attributeModifiersChunk)
        {
            for (var i = 0; i < attributeValuesChunk.Length; i++)
            {
                var attributeValues = attributeValuesChunk[i];
                var attributeModifierValues = attributeModifiersChunk[i];
                attributeValues.CurrentValue.Health = ModifyValues(attributeValues.BaseValue.Health, attributeModifierValues.AddValue.Health, attributeModifierValues.MultiplyValue.Health, attributeModifierValues.DivideValue.Health);
                attributeValues.CurrentValue.MaxHealth = ModifyValues(attributeValues.BaseValue.MaxHealth, attributeModifierValues.AddValue.MaxHealth, attributeModifierValues.MultiplyValue.MaxHealth, attributeModifierValues.DivideValue.MaxHealth);
                attributeValues.CurrentValue.Mana = ModifyValues(attributeValues.BaseValue.Mana, attributeModifierValues.AddValue.Mana, attributeModifierValues.MultiplyValue.Mana, attributeModifierValues.DivideValue.Mana);
                attributeValues.CurrentValue.MaxMana = ModifyValues(attributeValues.BaseValue.MaxMana, attributeModifierValues.AddValue.MaxMana, attributeModifierValues.MultiplyValue.MaxMana, attributeModifierValues.DivideValue.MaxMana);
                attributeValues.CurrentValue.Speed = ModifyValues(attributeValues.BaseValue.Speed, attributeModifierValues.AddValue.Speed, attributeModifierValues.MultiplyValue.Speed, attributeModifierValues.DivideValue.Speed);

                attributeValuesChunk[i] = attributeValues;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint ModifyValues(uint Base, float Add, float Multiply, float Divide)
        {
            return (uint)(((Base + Add) * (Multiply + 1)) / (Divide + 1));
        }
    }

    public class MyAttributeUpdateSystem : AttributeUpdateSystem<AttributeValues, MyAttributeModifierValues, MyPlayerAttributesJob>
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            var archetype = EntityManager.CreateArchetype(typeof(AttributeValues), typeof(MyAttributeModifierValues));
            for (var i = 0; i < 100; i++)
            {
                var entity = EntityManager.CreateEntity(archetype);
                SetComponent(entity, new AttributeValues()
                {
                    BaseValue = new MyPlayerAttributes<uint> { Health = 100, Mana = 10, MaxHealth = 100, MaxMana = 10, Speed = 5 }
                });

                SetComponent(entity, new MyAttributeModifierValues()
                {
                    AddValue = new MyPlayerAttributes<float> { Health = 10.0f, Mana = 5f },
                    MultiplyValue = new MyPlayerAttributes<float> { Health = 0.25f, Mana = 0.5f }
                });

            }
        }
    }
}

public class NativeStreamTestSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var stream = new NativeStream(5, Allocator.TempJob);
        var writeTransforms = new WriteTransforms { writer = stream.AsWriter() }.Schedule(5, 64);
        var handleResult = new HandleResult { reader = stream.AsReader() }.Schedule(5, 64, writeTransforms);
        stream.Dispose(handleResult);
    }

    struct WriteTransforms : IJobParallelFor
    {
        public NativeStream.Writer writer;
        public void Execute(int index)
        {
            writer.BeginForEachIndex(index);
            writer.Write<int>(index);
            writer.Write<int>(2 * index);
            writer.Write<int>(3 * index);
            writer.Write<int>(4 * index);
            writer.Write<int>(5 * index);
            writer.EndForEachIndex();
        }
    }

    struct HandleResult : IJobParallelFor
    {
        public NativeStream.Reader reader;

        public void Execute(int index)
        {
            int count = reader.BeginForEachIndex(index);
            for (int i = 0; i != count; i++)
            {
                var value = reader.Read<int>();
            }
            reader.EndForEachIndex();
        }
    }
}