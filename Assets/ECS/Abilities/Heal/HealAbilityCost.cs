using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealAbilityCost : ICost, IComponentData {
        public DurationPolicyComponent DurationPolicy { get; set; }
        const int ManaCost = 2;
        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            var attributeModData = new AttributeModificationComponent()
            {
                Add = -ManaCost,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };

            var attributeModEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, attributeModEntity, new ManaAttributeModifier());
            Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, attributeModData);
        }

        public void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            var attributeModData = new AttributeModificationComponent()
            {
                Add = -ManaCost,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };

            var attributeModEntity = EntityManager.CreateEntity(
                                            typeof(ManaAttributeModifier),
                                            typeof(PermanentAttributeModification),
                                            typeof(AttributeModificationComponent)
            );
            EntityManager.SetComponentData(attributeModEntity, attributeModData);
        }

        public AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes) {
            attributes.Mana.CurrentValue -= ManaCost;
            return attributes;
        }
    }
}