using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

/// <summary>
///    AbilitySystem is used to manage the entire ability system.
///    It is responsible for:
///        1. Checking to make sure unit has resource to cast
///        2. Checking to make sure ability is off cooldown
///        3. Checking to make sure unit is able to cast ability
///        4. Spawning meshes/running logic/animating required to cast ability
///            (and applying game effects such as costs)
/// </summary>
public class AbilitySystem<T> : JobComponentSystem
where T : struct, IComponentData, Ability {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [RequireComponentTag(typeof(CheckAbilityConstraints), typeof(CastingAbilityTagComponent))]
    public struct CheckAbilityConstraintsJob : IJobForEachWithEntity<T> {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public void Execute(Entity entity, int index, [ReadOnly] ref T ability) {
            if (attributesComponent.Exists(entity) &&
                ability.EntityHasResource(attributesComponent[entity])) {
                EntityCommandBuffer.AddComponent<PassedAbilityConstraintsComponent>(index, entity);
            }

            EntityCommandBuffer.RemoveComponent<CheckAbilityConstraints>(index, entity);
        }
    }
    public struct AbilityJob : IJobForEachWithEntity<T> {
        [ReadOnly] public ComponentDataFromEntity<AttributesComponent> attributesComponent;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref T ability) {
            // if (!attributesComponent.Exists(castingComponent.Owner)) return;

            // if (ability.EntityHasResource(attributesComponent[castingComponent.Owner])) {
            //     Debug.Log(entity);
            // }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job1 = new CheckAbilityConstraintsJob()
        {
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var job2 = new AbilityJob()
        {
            attributesComponent = GetComponentDataFromEntity<AttributesComponent>(true),
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        
        var jobHandle = job1.Schedule(this, inputDeps);
        var jobHandle2 = job2.Schedule(this, jobHandle);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle2);
        return jobHandle2;
    }

}

public class FireAbilitySystem : AbilitySystem<FireAbility> {

}

public struct CheckAbilityConstraints : IComponentData { }
public struct PassedAbilityConstraintsComponent : IComponentData { }

public struct CastingAbilityTagComponent : IComponentData { }
public struct FireAbility : Ability, IComponentData {
    public bool EntityHasResource(AttributesComponent attributes) {
        return attributes.Mana.CurrentValue >= 2;
    }
}

public interface Ability {
    bool EntityHasResource(AttributesComponent attributes);
}
