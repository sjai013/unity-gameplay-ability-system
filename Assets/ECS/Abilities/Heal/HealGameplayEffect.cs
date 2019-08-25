using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealGameplayEffect : IGameplayEffect, IComponentData {
        public DurationPolicyComponent DurationPolicy { get; set; }
        public Entity Target { get; set; }
        public Entity Source { get; set; }
        const float DamageAdder = +15;
        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            var attributeModData = AttributeModData();
            var attributeModEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, attributeModEntity, new HealthAttributeModifier());
            Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, attributeModData);
        }

        public void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            var attributeModData = AttributeModData();
            var attributeModEntity = EntityManager.CreateEntity(
                                                        typeof(HealthAttributeModifier),
                                                        typeof(PermanentAttributeModification),
                                                        typeof(AttributeModificationComponent)
            );
            EntityManager.SetComponentData(attributeModEntity, attributeModData);
        }
        private AttributeModificationComponent AttributeModData() {
            return new AttributeModificationComponent()
            {
                Add = DamageAdder,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };
        }

    }
}