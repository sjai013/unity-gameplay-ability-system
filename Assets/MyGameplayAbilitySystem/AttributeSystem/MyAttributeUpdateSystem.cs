using System.Runtime.CompilerServices;
using GameplayAbilitySystem.AttributeSystem.Systems;
using GameplayAbilitySystem.AttributeSystem.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem
{
    public class MyAttributeUpdateSystem : AttributeUpdateSystem<AttributeValues, MyAttributeModifierValues, MyPlayerAttributesJob, MyGameplayAttributeModifier>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            // Create dummy entities for testing
            var attributeArchetype = EntityManager.CreateArchetype(typeof(AttributeValues), typeof(MyAttributeModifierValues));
            var attributeModifierArchetype = EntityManager.CreateArchetype(typeof(GameplayEffectContext), typeof(MyGameplayAttributeModifier));
            CreateTestEntities(attributeArchetype, attributeModifierArchetype);
        }

        private void CreateTestEntities(EntityArchetype attributeArchetype, EntityArchetype attributeModifierArchetype)
        {
            var rand = new Random(1);

            for (var i = 0; i < 100; i++)
            {
                var entity = EntityManager.CreateEntity(attributeArchetype);
                SetComponent(entity, new AttributeValues()
                {
                    BaseValue = new MyPlayerAttributes<uint> { Health = 100, Mana = 10, MaxHealth = 100, MaxMana = 10, Speed = 5 }
                });

                for (var j = 0; j < 100; j++)
                {
                    var modifierEntity = EntityManager.CreateEntity(attributeModifierArchetype);
                    SetComponent(modifierEntity, new GameplayEffectContext()
                    {
                        Source = entity,
                        Target = entity
                    });
                    SetComponent(modifierEntity, new MyGameplayAttributeModifier()
                    {
                        Attribute = (EMyPlayerAttribute)(rand.NextInt(0, 5)),
                        Operator = (EMyAttributeModifierOperator)(rand.NextInt(1, 1)),
                        Value = (half)rand.NextFloat(0, 10)
                    });
                }

            }
        }
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