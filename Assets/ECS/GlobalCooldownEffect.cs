using Unity.Entities;

struct GlobalCooldownEffect : ICooldown, IComponentData {
    const float Duration = 2;
    public Entity Target { get; set; }
    public Entity Source { get; set; }
    public DurationPolicyComponent DurationPolicy { get; set; }

    public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime) {
        var attributeModData = new AttributeModificationComponent()
        {
            Add = 0,
            Multiply = 0,
            Divide = 0,
            Change = 0,
            Source = Source,
            Target = Target
        };

        var attributeModEntity = Ecb.CreateEntity(index);
        var gameplayEffectData = new GameplayEffectDurationComponent()
        {
            WorldStartTime = WorldTime,
            Duration = Duration,
            Effect = EGameplayEffect.GlobalCooldown
        };
        var cooldownEffectComponent = new CooldownEffectComponent()
        {
            Caster = Source
        };

        Ecb.AddComponent(index, attributeModEntity, new NullAttributeModifier());
        Ecb.AddComponent(index, attributeModEntity, new TemporaryAttributeModification());
        Ecb.AddComponent(index, attributeModEntity, gameplayEffectData);
        Ecb.AddComponent(index, attributeModEntity, attributeModData);
        Ecb.AddComponent(index, attributeModEntity, cooldownEffectComponent);
        Ecb.AddComponent(index, attributeModEntity, new GlobalCooldownTagComponent());
    }

    public void ApplyGameplayEffect(EntityManager EntityManager, AttributesComponent attributesComponent, float WorldTime) {
        throw new System.NotImplementedException();
    }
}

public struct GlobalCooldownTagComponent : IComponentData { }
