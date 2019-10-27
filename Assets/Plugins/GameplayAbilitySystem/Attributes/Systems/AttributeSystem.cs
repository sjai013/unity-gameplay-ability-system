using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.JobTypes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Systems {
    /// <summary>
    /// This is a generic attribute modification system which can be used
    /// out of the box, and supports single attribute modifications, using 
    /// Add, Multiply, and Divide operators.
    /// 
    /// The added, multiplied, and divided values are summed for each entity first, and then
    /// the calculation formula is applied to each entity: 
    /// <br/>
    /// <code>
    /// CurrentValue = Added + (BaseValue * [1 + multiplied - divided])
    /// </code>
    /// </summary>
    /// <typeparam name="TAttribute">The attribute this system modifies</typeparam>
    public abstract class GenericAttributeSystem<TAttributeTag> : AttributeModificationSystem<TAttributeTag>
where TAttributeTag : struct, IAttributeComponent, IComponentData {
        protected override void OnCreate() {
            this.Queries[0] = CreateQuery<Components.Operators.Add>();
            this.Queries[1] = CreateQuery<Components.Operators.Multiply>();
            this.Queries[2] = CreateQuery<Components.Operators.Divide>();

            var newEntity1 = CreatePlayer.CreatePlayerEntity(EntityManager);
            var newEntity2 = CreatePlayer.CreatePlayerEntity(EntityManager);

            CreateEntities<Components.Operators.Add, TAttributeTag>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Divide, TAttributeTag>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Multiply, TAttributeTag>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);

            this.actorsWithAttributesQuery = GetEntityQuery(
                ComponentType.ReadOnly<ActorWithAttributes>(),
                ComponentType.ReadWrite<TAttributeTag>()
                );
        }

        [BurstCompile]
        [RequireComponentTag(typeof(ActorWithAttributes))]
        struct AttributeCombinerJob : IJobForEachWithEntity<TAttributeTag> {
            [ReadOnly] public NativeMultiHashMap<Entity, float> AddAttributes;
            [ReadOnly] public NativeMultiHashMap<Entity, float> DivideAttributes;
            [ReadOnly] public NativeMultiHashMap<Entity, float> MultiplyAttributes;

            public void Execute(Entity entity, int index, ref TAttributeTag attribute) {
                var added = SumFromNMHM(entity, AddAttributes);
                var multiplied = SumFromNMHM(entity, MultiplyAttributes);
                var divided = SumFromNMHM(entity, DivideAttributes);

                attribute.CurrentValue = added + attribute.BaseValue * (1 + multiplied -  divided);
            }
            private float SumFromNMHM(Entity entity, NativeMultiHashMap<Entity, float> values) {
                values.TryGetFirstValue(entity, out var sum, out var multiplierIt);
                while (values.TryGetNextValue(out var tempSum, ref multiplierIt)) {
                    sum += tempSum;
                }
                return sum;
            }
        }
        protected override JobHandle ScheduleJobs(JobHandle inputDependencies) {
            ScheduleAttributeJob<Components.Operators.Add>(inputDependencies, this.Queries[0], out var AttributeHashAdd, out var addJob);
            ScheduleAttributeJob<Components.Operators.Multiply>(inputDependencies, this.Queries[1], out var AttributeHashMultiply, out var mulJob);
            ScheduleAttributeJob<Components.Operators.Divide>(inputDependencies, this.Queries[2], out var AttributeHashDivide, out var divideJob);
            inputDependencies = JobHandle.CombineDependencies(addJob, divideJob, mulJob);

            inputDependencies = new AttributeCombinerJob
            {
                AddAttributes = AttributeHashAdd,
                DivideAttributes = AttributeHashDivide,
                MultiplyAttributes = AttributeHashMultiply
            }.Schedule(this.actorsWithAttributesQuery, inputDependencies);

            inputDependencies = AttributeHashAdd.Dispose(inputDependencies);
            inputDependencies = AttributeHashMultiply.Dispose(inputDependencies);
            inputDependencies = AttributeHashDivide.Dispose(inputDependencies);

            return inputDependencies;
        }

        private void ScheduleAttributeJob<TOper>(JobHandle inputDependencies, EntityQuery query, out NativeMultiHashMap<Entity, float> AttributeHash, out JobHandle job)
        where TOper : struct, IAttributeOperator, IComponentData {
            AttributeHash = new NativeMultiHashMap<Entity, float>(query.CalculateEntityCount(), Allocator.TempJob);
            job = new GetAttributeValuesJob_Sum<TOper, TAttributeTag>
            {
                AttributeModifierValues = AttributeHash.AsParallelWriter()
            }.Schedule(query, inputDependencies);
        }


    }
}


internal class CreateEntities<TOper, TAttribute>
where TOper : struct, IAttributeOperator, IComponentData
where TAttribute : struct, IAttributeComponent, IComponentData {
    public static void CreateAttributeOperEntities(EntityManager EntityManager, Entity Entity1, Entity Entity2) {

        var archetype = EntityManager.CreateArchetype(
            typeof(TOper),
            typeof(AttributeComponentTag<TAttribute>),
            typeof(AttributeModifier<TOper, TAttribute>),
            typeof(AttributesOwnerComponent)
        );

        for (var i = 0; i < 0; i++) {
            var entity = EntityManager.CreateEntity(archetype);
            EntityManager.SetComponentData(entity, new GameplayAbilitySystem.Attributes.Components.AttributeModifier<TOper, TAttribute>()
            {
                Value = 1
            });

            EntityManager.SetComponentData(entity, new AttributesOwnerComponent()
            {
                Value = i % 2 == 0 ? Entity1 : Entity2
            });
        }
    }
}

internal class CreatePlayer {
    public static Entity CreatePlayerEntity(EntityManager EntityManager) {
        var playerArchetype = EntityManager.CreateArchetype(
            typeof(HealthAttributeComponent),
            typeof(ActorWithAttributes)
        );

        return EntityManager.CreateEntity(playerArchetype);
    }
}