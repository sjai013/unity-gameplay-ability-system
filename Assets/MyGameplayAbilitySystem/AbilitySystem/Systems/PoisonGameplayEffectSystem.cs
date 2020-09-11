using GameplayAbilitySystem.AbilitySystem.GameplayEffects.Components;
using GameplayAbilitySystem.AbilitySystem.GameplayEffects.Systems;
using GameplayAbilitySystem.AttributeSystem.Components;
using MyGameplayAbilitySystem;
using Unity.Entities;
using Unity.Mathematics;

public class PoisonGameplayEffectSystem : GameplayEffectActionSystem<PoisonGameplayEffect>
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    EntityArchetype m_PoisonGameplayEffectArchetype;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_PoisonGameplayEffectArchetype = MyInstantAttributeUpdateSystem.CreateModifierArchetype(EntityManager);
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();


    }
    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
        var archetype = m_PoisonGameplayEffectArchetype;
        Entities.ForEach((int entityInQueryIndex, Entity entity, PoisonGameplayEffect e, DurationStateComponent durationState, GameplayEffectContextComponent context) =>
        {
            if (!durationState.TickedThisFrame()) return;
            Entity attributeEntity = ecb.CreateEntity(entityInQueryIndex, archetype);
            ecb.SetComponent(entityInQueryIndex, attributeEntity, new MyInstantGameplayAttributeModifier()
            {
                Attribute = EMyPlayerAttribute.Health,
                Operator = EMyAttributeModifierOperator.Add,
                Value = (half)(-e.DamagePerTick)
            });
            ecb.SetComponent(entityInQueryIndex, attributeEntity, new GameplayEffectContextComponent()
            {
                Source = context.Source,
                Target = context.Target
            });
        }).ScheduleParallel();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}