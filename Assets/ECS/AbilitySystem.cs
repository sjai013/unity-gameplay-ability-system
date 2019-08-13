using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

internal struct CheckAbilityConstraintsComponent : IComponentData { }
internal struct CommitJobComponent : IComponentData { }
internal struct CommitJobSystemComponent : ISystemStateComponentData { }
internal struct CastingAbilityTagComponent : IComponentData { }

internal struct CancelAbilityComponent : IComponentData { }

/// <summary>
///    AbilitySystem is used to manage the entire ability system.
///    It is responsible for:
///        1. Checking to make sure unit has resource to cast
///        2. Checking to make sure ability is off cooldown
///        3. Checking to make sure unit is able to cast ability
///        4. Spawning meshes/running logic/animating required to cast ability
///            (and applying game effects such as costs)
/// </summary>
public class AbilitySystem<TAbility, TCost> : JobComponentSystem 
where TAbility : struct, IComponentData, IAbility 
where TCost : struct, IComponentData, ICost

{
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }
    [RequireComponentTag(typeof(CheckAbilityConstraintsComponent))]
    public struct CheckAbilityConstraintsJob : IJobForEachWithEntity<TAbility, TCost> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability, [ReadOnly] ref TCost cost) {
            var source = ability.Source;
            if (attributesComponent.Exists(source) &&
                cost.CheckResourcesAvailable(source, attributesComponent[source])) {
                EntityCommandBuffer.AddComponent<CommitJobComponent>(index, entity);
            } else {
                EntityCommandBuffer.AddComponent<CancelAbilityComponent>(index, entity);
            }
            EntityCommandBuffer.RemoveComponent<CheckAbilityConstraintsComponent>(index, entity);
        }
    }

    [RequireComponentTag(typeof(CommitJobComponent))]
    public struct CommitAbilityJob : IJobForEachWithEntity<TAbility, TCost> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability, [ReadOnly] ref TCost Cost) {
            var attributes = attributesComponent[ability.Source];
            Cost.ApplyGameplayEffect(index, EntityCommandBuffer, ability.Source, ability.Source, attributes);
            EntityCommandBuffer.RemoveComponent<CommitJobComponent>(index, entity);
            EntityCommandBuffer.RemoveComponent<CastingAbilityTagComponent>(index, ability.Source);
        }

    }

    [RequireComponentTag(typeof(CancelAbilityComponent))]
    public struct CancelAbilityJob : IJobForEachWithEntity<TAbility> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility _) {
            EntityCommandBuffer.DestroyEntity(index, entity);
        }
    }

    public struct AbilityJob : IJobForEachWithEntity<TAbility> {
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref TAbility ability) {
            // if (!attributesComponent.Exists(castingComponent.Owner)) return;

            // if (ability.EntityHasResource(attributesComponent[castingComponent.Owner])) {
            //     Debug.Log(entity);
            // }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var job1 = new CheckAbilityConstraintsJob()
        {
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = commandBuffer
        };

        var job2 = new CommitAbilityJob()
        {
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = commandBuffer
    };

        var jobCancelAbility = new CancelAbilityJob()
        {
            EntityCommandBuffer = commandBuffer
        };

        var jobHandle = job1.Schedule(this, inputDeps);
        var jobHandle2 = job2.Schedule(this, jobHandle);

        var jobCancelAbilityHandle = jobCancelAbility.Schedule(this, jobHandle2);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobCancelAbilityHandle);
        return jobCancelAbilityHandle;
    }
}

public interface IAbility {
    Entity Target { get; set; }
    Entity Source { get; set; }
}

public interface IGameplayEffect {
    void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
}

public interface ICost: IGameplayEffect {
    bool CheckResourcesAvailable(Entity Caster, AttributesComponent attributes);
}