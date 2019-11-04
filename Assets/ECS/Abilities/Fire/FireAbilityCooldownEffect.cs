using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityCooldownEffect : ICooldown, IComponentData {
        const float Duration = 5f;

        public Entity Target { get; set; }
        public Entity Source { get; set; }
        public DurationPolicyComponent DurationPolicy { get; set; }

        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime) {
            var attributeModData = new _AttributeModificationComponent()
            {
                Add = 0,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };

            var attributeModEntity = Ecb.CreateEntity(index);
            var gameplayEffectData = new _GameplayEffectDurationComponent()
            {
                WorldStartTime = WorldTime,
                Duration = Duration,
                Effect = EGameplayEffect.FireAbilityCooldown
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
        }

        public void ApplyGameplayEffect(EntityManager EntityManager, AttributesComponent attributesComponent, float WorldTime) {
            throw new System.NotImplementedException();
        }
    }
}