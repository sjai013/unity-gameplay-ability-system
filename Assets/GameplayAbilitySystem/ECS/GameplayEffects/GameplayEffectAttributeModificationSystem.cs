using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;


public class GameplayEffectAttributeModificationSystem : JobComponentSystem {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [RequireComponentTag(typeof(AttributeModifyComponent))]
    struct Job : IJobForEachWithEntity<TemporaryAttributeModificationComponent> {
        public EntityCommandBuffer.Concurrent Ecb;
        [NativeDisableContainerSafetyRestriction] [WriteOnly] public ComponentDataFromEntity<AttributesComponent> attrComponents;
        public void Execute(Entity entity, int index, [ReadOnly] ref TemporaryAttributeModificationComponent attrMod) {
            if (attrComponents.Exists(attrMod.Target)) {
                var attrs = attrComponents[attrMod.Target];
                attrs.Health.CurrentValue += attrMod.Change; ;
                attrComponents[attrMod.Target] = attrs;
            }

            Ecb.RemoveComponent<AttributeModifyComponent>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new Job() {
            Ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            attrComponents = GetComponentDataFromEntity<AttributesComponent>(false)
        };

        var jobHandle = job.Schedule(this, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;

    }
}

public class GameplayEffectAttributeModificationUndoSystem : JobComponentSystem {
    BeginSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate() {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [RequireComponentTag(typeof(GameplayEffectExpired))]
    [ExcludeComponent(typeof(AttributeModificationUndoAppliedComponent))]
    struct Job : IJobForEachWithEntity<TemporaryAttributeModificationComponent> {
        public EntityCommandBuffer.Concurrent Ecb;
        [NativeDisableContainerSafetyRestriction] [WriteOnly] public ComponentDataFromEntity<AttributesComponent> attrComponents;
        public void Execute(Entity entity, int index, [ReadOnly] ref TemporaryAttributeModificationComponent attrMod) {
            if (attrComponents.Exists(attrMod.Target)) {
                var attrs = attrComponents[attrMod.Target];
                attrs.Health.CurrentValue -= attrMod.Change; ;
                attrComponents[attrMod.Target] = attrs;
            }

            Ecb.AddComponent(index, entity, new AttributeModificationUndoAppliedComponent());
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new Job() {
            Ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            attrComponents = GetComponentDataFromEntity<AttributesComponent>(false)
        };

        var jobHandle = job.Schedule(this, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;

    }
}