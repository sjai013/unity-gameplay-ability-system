using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.JobTypes;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {
    internal class CreateEntities<TOper, TAttribute>
where TOper : struct, IAttributeOperator, IComponentData
where TAttribute : struct, IAttributeComponent, IComponentData {
        public static void Create(EntityManager EntityManager, Entity Entity1, Entity Entity2) {

            var archetype = EntityManager.CreateArchetype(
                typeof(TAttribute),
                typeof(TOper),
                typeof(AttributeModifier<TOper, TAttribute>),
                typeof(AttributesOwnerComponent)
            );


            for (var i = 0; i < 10000; i++) {
                var entity = EntityManager.CreateEntity(archetype);
                EntityManager.SetComponentData(entity, new Components.AttributeModifier<TOper, TAttribute>()
                {
                    Value = 100
                });

                EntityManager.SetComponentData(entity, new AttributesOwnerComponent()
                {
                    Value = i % 2 == 0 ? Entity1 : Entity2
                });
            }
        }
    }

    public abstract class GenericAttributeSystem<TAttributeTag> : AttributeModificationSystem<TAttributeTag>
    where TAttributeTag : struct, IAttributeComponent, IComponentData {
        protected override void OnCreate() {
            this.Queries = new List<EntityQuery>();
            this.Queries.Add(CreateQuery<Components.Operators.Add>());
            this.Queries.Add(CreateQuery<Components.Operators.Multiply>());
            this.Queries.Add(CreateQuery<Components.Operators.Divide>());

            var newEntity1 = EntityManager.CreateEntity();
            var newEntity2 = EntityManager.CreateEntity();

            CreateEntities<Components.Operators.Add, TAttributeTag>.Create(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Divide, TAttributeTag>.Create(EntityManager, newEntity1, newEntity2);
            CreateEntities<Components.Operators.Multiply, TAttributeTag>.Create(EntityManager, newEntity1, newEntity2);

        }
        protected override JobHandle ScheduleJobs(JobHandle inputDependencies) {
            //throw new System.NotImplementedException();

            var AttributeHashAdd = new NativeHashMap<Entity, float>(this.Queries[0].CalculateEntityCount(), Allocator.TempJob);
            var addJob = new GetAttributeValuesJob_Sum<Components.Operators.Add, TAttributeTag>
            {
                AttributeModifierValues = AttributeHashAdd,
                attributeModifiers = this.Queries[0].ToComponentDataArray<AttributeModifier<Components.Operators.Add, TAttributeTag>>(Allocator.TempJob),
                attributeOwners = this.Queries[0].ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDependencies);

            var AttributeHashMultiply = new NativeHashMap<Entity, float>(this.Queries[1].CalculateEntityCount(), Allocator.TempJob);
            var mulJob = new GetAttributeValuesJob_Sum<Components.Operators.Multiply, TAttributeTag>
            {
                AttributeModifierValues = AttributeHashMultiply,
                attributeModifiers = this.Queries[1].ToComponentDataArray<AttributeModifier<Components.Operators.Multiply, TAttributeTag>>(Allocator.TempJob),
                attributeOwners = this.Queries[1].ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDependencies);

            var AttributeHashDivide = new NativeHashMap<Entity, float>(this.Queries[2].CalculateEntityCount(), Allocator.TempJob);
            var divideJob = new GetAttributeValuesJob_Sum<Components.Operators.Divide, TAttributeTag>
            {
                AttributeModifierValues = AttributeHashDivide,
                attributeModifiers = this.Queries[2].ToComponentDataArray<AttributeModifier<Components.Operators.Divide, TAttributeTag>>(Allocator.TempJob),
                attributeOwners = this.Queries[2].ToComponentDataArray<AttributesOwnerComponent>(Allocator.TempJob)
            }.Schedule(inputDependencies);

            inputDependencies = JobHandle.CombineDependencies(addJob, divideJob, mulJob);
            inputDependencies = AttributeHashAdd.Dispose(inputDependencies);
            inputDependencies = AttributeHashMultiply.Dispose(inputDependencies);
            inputDependencies = AttributeHashDivide.Dispose(inputDependencies);

            return inputDependencies;
        }

    }
}