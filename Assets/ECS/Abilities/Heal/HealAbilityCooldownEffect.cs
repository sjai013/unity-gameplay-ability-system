using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealAbilityCooldownEffect : ICooldown, IComponentData {
        const float Duration = 3f;
        public Entity Target { get; set; }
        public Entity Source { get; set; }
        public DurationPolicyComponent DurationPolicy { get; set; }

        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, float WorldTime) {
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
                Effect = EGameplayEffect.HealAbilityCooldown
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
                Effect = EGameplayEffect.HealAbilityCooldown
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