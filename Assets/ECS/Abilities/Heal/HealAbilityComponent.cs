using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealAbilityComponent : IAbility, IComponentData {
        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new HealAbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }

        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            new AbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
            new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        }

        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new HealGameplayEffect().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }

        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new HealGameplayEffect().ApplyGameplayEffect(entityManager, Source, Target, attributesComponent);
        }

        public bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes) {
            attributes = new HealAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }

        public struct AbilityCooldownEffect : ICooldown, IComponentData {
            const float Duration = 3f;
            public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
                var attributeModData = new AttributeModificationComponent()
                {
                    Add = 0,
                    Multiply = 0,
                    Divide = 0,
                    Change = 0,
                    Source = Caster,
                    Target = Caster
                };

                var attributeModEntity = Ecb.CreateEntity(index);
                var gameplayEffectData = new GameplayEffectDurationComponent()
                {
                    WorldStartTime = WorldTime,
                    Duration = Duration,
                };
                var cooldownEffectComponent = new CooldownEffectComponent()
                {
                    Caster = Caster
                };

                Ecb.AddComponent(index, attributeModEntity, new NullAttributeModifier());
                Ecb.AddComponent(index, attributeModEntity, new TemporaryAttributeModification());
                Ecb.AddComponent(index, attributeModEntity, gameplayEffectData);
                Ecb.AddComponent(index, attributeModEntity, attributeModData);
                Ecb.AddComponent(index, attributeModEntity, cooldownEffectComponent);
                Ecb.AddComponent(index, attributeModEntity, new HealCooldownTagComponent());
            }
        }
    }
}