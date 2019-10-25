using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.JobTypes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Systems {
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

            for (var i = 0; i < 100; i++) {
                var entity = EntityManager.CreateEntity(archetype);
                EntityManager.SetComponentData(entity, new Components.AttributeModifier<TOper, TAttribute>()
                {
                    Value = 1
                });

                EntityManager.SetComponentData(entity, new AttributesOwnerComponent()
                {
                    Value = i % 2 == 0 ? Entity1 : Entity2
                });
            }
        }

        public static Entity CreatePlayerEntity(EntityManager EntityManager) {
            var playerArchetype = EntityManager.CreateArchetype(
                typeof(HealthAttributeComponent),
                typeof(ActorWithAttributesTag)
            );

            return EntityManager.CreateEntity(playerArchetype);
        }
    }

    internal class CreatePlayer {
        public static Entity CreatePlayerEntity(EntityManager EntityManager) {
            var playerArchetype = EntityManager.CreateArchetype(
                typeof(HealthAttributeComponent),
                typeof(ActorWithAttributesTag)
            );

            return EntityManager.CreateEntity(playerArchetype);
        }
    }

    public abstract class GenericAttributeSystem<TAttribute> : AttributeModificationSystem<TAttribute>
    where TAttribute : struct, IAttributeComponent, IComponentData {

        protected override void OnCreate() {
            this.Queries = new List<EntityQuery>();
            this.Queries.Add(CreateQuery<Components.Operators.Add>());
            this.Queries.Add(CreateQuery<Components.Operators.Multiply>());
            this.Queries.Add(CreateQuery<Components.Operators.Divide>());

            var newEntity1 = CreatePlayer.CreatePlayerEntity(EntityManager);
            var newEntity2 = CreatePlayer.CreatePlayerEntity(EntityManager);

            CreateEntities<Components.Operators.Add, TAttribute>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Divide, TAttribute>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Multiply, TAttribute>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);

        }

        [BurstCompile]
        struct AttributeCombinerJob : IJob {
            [ReadOnly] public NativeHashMap<Entity, float> AddAttributes;
            [ReadOnly] public NativeHashMap<Entity, float> DivideAttributes;
            [ReadOnly] public NativeHashMap<Entity, float> MultiplyAttributes;
            public void Execute() {
                // Get list of unique entities from all the hashmaps
            }
        }

        protected override JobHandle ScheduleJobs(JobHandle inputDependencies) {
            var AttributeHashAdd = new NativeHashMap<Entity, float>(this.Queries[0].CalculateEntityCount(), Allocator.TempJob);
            var addJob = new GetAttributeValuesJob_Sum1<Components.Operators.Add, TAttribute>
            {
                AttributeModifierValues = AttributeHashAdd,
                attributeModifiers = this.Queries[0].ToComponentDataArray<AttributeModifier<Components.Operators.Add, TAttribute>>(Allocator.TempJob),
                attributeOwners = this.Queries[0].ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDependencies);

            var AttributeHashMultiply = new NativeHashMap<Entity, float>(this.Queries[1].CalculateEntityCount(), Allocator.TempJob);
            var mulJob = new GetAttributeValuesJob_Sum1<Components.Operators.Multiply, TAttribute>
            {
                AttributeModifierValues = AttributeHashMultiply,
                attributeModifiers = this.Queries[1].ToComponentDataArray<AttributeModifier<Components.Operators.Multiply, TAttribute>>(Allocator.TempJob),
                attributeOwners = this.Queries[1].ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDependencies);

            var AttributeHashDivide = new NativeHashMap<Entity, float>(this.Queries[2].CalculateEntityCount(), Allocator.TempJob);
            var divideJob = new GetAttributeValuesJob_Sum1<Components.Operators.Divide, TAttribute>
            {
                AttributeModifierValues = AttributeHashDivide,
                attributeModifiers = this.Queries[2].ToComponentDataArray<AttributeModifier<Components.Operators.Divide, TAttribute>>(Allocator.TempJob),
                attributeOwners = this.Queries[2].ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDependencies);

            inputDependencies = JobHandle.CombineDependencies(addJob, divideJob, mulJob);
            inputDependencies = new AttributeCombinerJob
            {
                AddAttributes = AttributeHashAdd,
                DivideAttributes = AttributeHashDivide,
                MultiplyAttributes = AttributeHashMultiply
            }.Schedule(inputDependencies);

            inputDependencies = AttributeHashAdd.Dispose(inputDependencies);
            inputDependencies = AttributeHashMultiply.Dispose(inputDependencies);
            inputDependencies = AttributeHashDivide.Dispose(inputDependencies);

            return inputDependencies;
        }

    }


    public abstract class GenericAttributeSystem2<TAttributeTag> : AttributeModificationSystem<TAttributeTag>
where TAttributeTag : struct, IAttributeComponent, IComponentData {

        protected override void OnCreate() {
            this.Queries = new List<EntityQuery>();
            this.Queries.Add(CreateQuery<Components.Operators.Add>());
            this.Queries.Add(CreateQuery<Components.Operators.Multiply>());
            this.Queries.Add(CreateQuery<Components.Operators.Divide>());

            var newEntity1 = CreatePlayer.CreatePlayerEntity(EntityManager);
            var newEntity2 = CreatePlayer.CreatePlayerEntity(EntityManager);

            CreateEntities<Components.Operators.Add, TAttributeTag>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Divide, TAttributeTag>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Multiply, TAttributeTag>.CreateAttributeOperEntities(EntityManager, newEntity1, newEntity2);

            this.actorsWithAttributesQuery = GetEntityQuery(
                ComponentType.ReadOnly<ActorWithAttributesTag>(),
                ComponentType.ReadWrite<TAttributeTag>()
                );
        }

        [BurstCompile]
        [RequireComponentTag(typeof(ActorWithAttributesTag))]
        struct AttributeCombinerJob : IJobForEachWithEntity<TAttributeTag> {
            [ReadOnly] public NativeMultiHashMap<Entity, float> AddAttributes;
            [ReadOnly] public NativeMultiHashMap<Entity, float> DivideAttributes;
            [ReadOnly] public NativeMultiHashMap<Entity, float> MultiplyAttributes;

            public void Execute(Entity entity, int index, ref TAttributeTag attribute) {
                var added = SumFromNMHM(entity, AddAttributes);
                var multiplied = SumFromNMHM(entity, MultiplyAttributes);
                var divided = SumFromNMHM(entity, DivideAttributes);
                if (divided == 0) divided = 1;

                attribute.CurrentValue = attribute.BaseValue + added + attribute.BaseValue * (multiplied / divided);
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

            var AttributeHashAdd2 = new NativeMultiHashMap<Entity, float>(this.Queries[0].CalculateEntityCount(), Allocator.TempJob);
            var addJob = new GetAttributeValuesJob_Sum2<Components.Operators.Add, TAttributeTag>
            {
                AttributeModifierValues = AttributeHashAdd2.AsParallelWriter()
            }.Schedule(this.Queries[0], inputDependencies);

            var AttributeHashMultiply2 = new NativeMultiHashMap<Entity, float>(this.Queries[1].CalculateEntityCount(), Allocator.TempJob);
            var mulJob = new GetAttributeValuesJob_Sum2<Components.Operators.Multiply, TAttributeTag>
            {
                AttributeModifierValues = AttributeHashMultiply2.AsParallelWriter(),
            }.Schedule(this.Queries[1], inputDependencies);

            var AttributeHashDivide2 = new NativeMultiHashMap<Entity, float>(this.Queries[2].CalculateEntityCount(), Allocator.TempJob);
            var divideJob = new GetAttributeValuesJob_Sum2<Components.Operators.Divide, TAttributeTag>
            {
                AttributeModifierValues = AttributeHashDivide2.AsParallelWriter(),
            }.Schedule(this.Queries[2], inputDependencies);

            inputDependencies = JobHandle.CombineDependencies(addJob, divideJob, mulJob);

            inputDependencies = new AttributeCombinerJob
            {
                AddAttributes = AttributeHashAdd2,
                DivideAttributes = AttributeHashDivide2,
                MultiplyAttributes = AttributeHashMultiply2
            }.Schedule(this.actorsWithAttributesQuery, inputDependencies);

            inputDependencies = AttributeHashAdd2.Dispose(inputDependencies);
            inputDependencies = AttributeHashMultiply2.Dispose(inputDependencies);
            inputDependencies = AttributeHashDivide2.Dispose(inputDependencies);

            return inputDependencies;
        }

    }
}