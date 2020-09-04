using System.Runtime.CompilerServices;
using GameplayAbilitySystem.AttributeSystem.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MyGameplayAbilitySystem
{

    public class AttributeModifierCollectorSystem : SystemBase
    // where T1 : struct, ICompnentData, IGameplayAttributeModifier<T2>
    // where T2 : struct, IComponentData, IAttributeModifier
    {
        private EntityQuery m_AttributeModifiers;
        private EntityQuery m_AttributesGroup;
        protected override void OnCreate()
        {
            m_AttributeModifiers = GetEntityQuery(typeof(MyGameplayAttributeModifier));
            m_AttributesGroup = GetEntityQuery(typeof(MyAttributeModifierValues));
        }
        protected override void OnUpdate()
        {

            // 1. Gather all attribute modifier components
            // EITHER:
            // 2a. Modify value of TAttributeModifier component to reflect the mods
            // OR
            // 2b. Keep those values in memory (NativeStream, NativeMultiHashMap, w/e), and apply directly to attributes
            //NativeHashMap<Entity, MyGameplayAttributeModifier> a = new NativeHashMap<Entity, MyGameplayAttributeModifier>(m_AttributeModifiers.CalculateEntityCount(), Allocator.TempJob);
            NativeMultiHashMap<Entity, MyGameplayAttributeModifier> b = new NativeMultiHashMap<Entity, MyGameplayAttributeModifier>(m_AttributeModifiers.CalculateEntityCount(), Allocator.TempJob);
            var j = new AJob()
            {
                attributeWriter = b.AsParallelWriter(),
                GameplayAttributeModifierHandle = GetComponentTypeHandle<MyGameplayAttributeModifier>(true),
                GameplayEffectContextHandle = GetComponentTypeHandle<GameplayEffectContext>(true)
            };

            Dependency = j.ScheduleParallel(m_AttributeModifiers, Dependency);
            Dependency = new BJob()
            {
                AttributeModifierValuesHandle = GetComponentTypeHandle<MyAttributeModifierValues>(false),
                attributesGroup = b,
                entitiesHandle = GetEntityTypeHandle()
            }.Schedule(m_AttributesGroup, Dependency);



            // Now write to the attributes


            b.Dispose(Dependency);
        }

        struct AJob : IJobChunk
        {
            public NativeMultiHashMap<Entity, MyGameplayAttributeModifier>.ParallelWriter attributeWriter;
            [ReadOnly] public ComponentTypeHandle<MyGameplayAttributeModifier> GameplayAttributeModifierHandle;
            [ReadOnly] public ComponentTypeHandle<GameplayEffectContext> GameplayEffectContextHandle;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var attributeModifierChunk = chunk.GetNativeArray(GameplayAttributeModifierHandle);
                var gameplayEffectContextChunk = chunk.GetNativeArray(GameplayEffectContextHandle);
                for (var i = 0; i < chunk.Count; i++)
                {
                    var attributeModifier = attributeModifierChunk[i];
                    var gameplayEffectContext = gameplayEffectContextChunk[i];

                    attributeWriter.Add(gameplayEffectContext.Target, attributeModifier);
                }

            }
        }

        [BurstCompile]
        struct BJob : IJobChunk
        {
            public ComponentTypeHandle<MyAttributeModifierValues> AttributeModifierValuesHandle;
            [ReadOnly] public NativeMultiHashMap<Entity, MyGameplayAttributeModifier> attributesGroup;

            [ReadOnly] public EntityTypeHandle entitiesHandle;
            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var attributeModifierValuesChunk = chunk.GetNativeArray(AttributeModifierValuesHandle);
                var entityChunk = chunk.GetNativeArray(entitiesHandle);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = entityChunk[i];
                    MyAttributeModifierValues attributeModifierValue = new MyAttributeModifierValues();
                    if (attributesGroup.TryGetFirstValue(entity, out var modifier, out var iterator))
                    {
                        modifier.UpdateAttribute(ref attributeModifierValue);
                        while (attributesGroup.TryGetNextValue(out modifier, ref iterator))
                        {
                            modifier.UpdateAttribute(ref attributeModifierValue);
                        }
                    }

                    attributeModifierValuesChunk[i] = attributeModifierValue;

                }

            }
        }

        [BurstCompile]
        struct CJob : IJob
        {
            public NativeArray<Entity> entities;
            public NativeArray<MyGameplayAttributeModifier> attributeModifiers;

            [NativeDisableParallelForRestriction]
            public ComponentDataFromEntity<MyAttributeModifierValues> PlayerAttributeModifiersFromEntity;

            public void Execute()
            {
                for (var i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    var modifier = attributeModifiers[i];
                    var playerModifier = PlayerAttributeModifiersFromEntity[entity];

                    modifier.UpdateAttribute(ref playerModifier);
                    PlayerAttributeModifiersFromEntity[entity] = playerModifier;
                }
            }
        }
    }

    public class MyAttributeUpdateSystem : AttributeUpdateSystem<AttributeValues, MyAttributeModifierValues, MyPlayerAttributesJob>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            var rand = new Random(1);
            // Create dummy entities for testing
            var attributeArchetype = EntityManager.CreateArchetype(typeof(AttributeValues), typeof(MyAttributeModifierValues));
            var attributeModifierArchetype = EntityManager.CreateArchetype(typeof(GameplayEffectContext), typeof(MyGameplayAttributeModifier));
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