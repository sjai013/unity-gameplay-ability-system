using System.Runtime.CompilerServices;
using GameplayAbilitySystem.AttributeSystem.Systems;
using GameplayAbilitySystem.AttributeSystem.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem
{
    public class MyAttributeUpdateSystem : AttributeUpdateSystem<AttributeValues, MyInstantAttributeModifierValues, MyDurationalAttributeModifierValues, MyPlayerAttributesJob>
    {
        public static Entity CreateGameplayEffect(EntityManager dstManager, GameplayEffectContextComponent context, MyDurationalGameplayAttributeModifier modifier)
        {
            var attributeModifierArchetype = dstManager.CreateArchetype(typeof(GameplayEffectContextComponent), typeof(MyDurationalGameplayAttributeModifier));
            var entity = dstManager.CreateEntity(attributeModifierArchetype);
            dstManager.SetComponentData(entity, context);
            dstManager.SetComponentData(entity, modifier);
            return entity;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            // Create dummy entities for testing
            //CreateTestEntities(attributeArchetype, attributeModifierArchetype);
        }

        private void CreateTestEntities(EntityArchetype attributeArchetype, EntityArchetype attributeModifierArchetype)
        {
            var rand = new Random(1);

            for (var i = 0; i < 100; i++)
            {
                var defaultAttributes = new AttributeValues()
                {
                    BaseValue = new MyPlayerAttributes<uint> { Health = 100, Mana = 10, MaxHealth = 100, MaxMana = 10, Speed = 5 }
                };

                var entity = CreatePlayerEntity(EntityManager, defaultAttributes);

                for (var j = 0; j < 100; j++)
                {
                    var context = new GameplayEffectContextComponent()
                    {
                        Source = entity,
                        Target = entity
                    };

                    var attributesModifier = new MyDurationalGameplayAttributeModifier()
                    {
                        Attribute = (EMyPlayerAttribute)(rand.NextInt(0, 5)),
                        Operator = (EMyAttributeModifierOperator)(rand.NextInt(1, 1)),
                        Value = (half)rand.NextFloat(0, 10)
                    };

                    CreateGameplayEffect(EntityManager, context, attributesModifier);
                }

            }
        }
    }

    public struct MyPlayerAttributesJob : IAttributeExecute<AttributeValues, MyInstantAttributeModifierValues, MyDurationalAttributeModifierValues>
    {
        public void CalculateDurational(NativeArray<AttributeValues> attributeValuesChunk, NativeArray<MyDurationalAttributeModifierValues> attributeModifiersChunk)
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
                ClampAttributes(ref attributeValues.CurrentValue);
                attributeValuesChunk[i] = attributeValues;
            }
        }
        public void CalculateInstant(NativeArray<AttributeValues> attributeValuesChunk, NativeArray<MyInstantAttributeModifierValues> attributeModifiersChunk)
        {
            for (var i = 0; i < attributeValuesChunk.Length; i++)
            {
                var attributeValues = attributeValuesChunk[i];
                var attributeModifierValues = attributeModifiersChunk[i];
                attributeValues.BaseValue.Health = ModifyValues(attributeValues.BaseValue.Health, attributeModifierValues.AddValue.Health, attributeModifierValues.MultiplyValue.Health, attributeModifierValues.DivideValue.Health);
                attributeValues.BaseValue.MaxHealth = ModifyValues(attributeValues.BaseValue.MaxHealth, attributeModifierValues.AddValue.MaxHealth, attributeModifierValues.MultiplyValue.MaxHealth, attributeModifierValues.DivideValue.MaxHealth);
                attributeValues.BaseValue.Mana = ModifyValues(attributeValues.BaseValue.Mana, attributeModifierValues.AddValue.Mana, attributeModifierValues.MultiplyValue.Mana, attributeModifierValues.DivideValue.Mana);
                attributeValues.BaseValue.MaxMana = ModifyValues(attributeValues.BaseValue.MaxMana, attributeModifierValues.AddValue.MaxMana, attributeModifierValues.MultiplyValue.MaxMana, attributeModifierValues.DivideValue.MaxMana);
                attributeValues.BaseValue.Speed = ModifyValues(attributeValues.BaseValue.Speed, attributeModifierValues.AddValue.Speed, attributeModifierValues.MultiplyValue.Speed, attributeModifierValues.DivideValue.Speed);
                ClampAttributes(ref attributeValues.BaseValue);
                attributeValuesChunk[i] = attributeValues;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint ModifyValues(uint Base, float Add, float Multiply, float Divide)
        {
            return (uint)(((Base + Add) * (Multiply + 1)) / (Divide + 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ClampAttributes(ref MyPlayerAttributes<uint> Attributes)
        {
            Attributes.MaxHealth = math.max(0, Attributes.MaxHealth);
            Attributes.MaxMana = math.max(0, Attributes.MaxMana);
            Attributes.Speed = math.max(0, Attributes.Speed);
            Attributes.Health = math.clamp(Attributes.Health, 0, Attributes.MaxHealth);
            Attributes.Mana = math.clamp(Attributes.Mana, 0, Attributes.MaxMana);
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
}
