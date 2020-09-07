using GameplayAbilitySystem.AttributeSystem.Components;
using GameplayAbilitySystem.AttributeSystem.Systems;
using Unity.Entities;

[UpdateAfter(typeof(AttributeUpdateSystemGroup))]
public class InstantAttributeSystemCleanup : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref InstantAttributeModifierComponentTag tag) =>
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}