using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityCost : ICost, IComponentData {

        public Entity Target { get; set; }
        public Entity Source { get; set; }
        const int ManaCost = 2;
        public DurationPolicyComponent DurationPolicy { get; set; }
        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime) {

            var attributeModData = AttributeModData(this.Source, this.Source);
            var attributeModEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, attributeModEntity, new ManaAttributeModifier());
            Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, attributeModData);
        }

        public void ApplyGameplayEffect(EntityManager EntityManager, AttributesComponent attributesComponent, float WorldTime) {
            var attributeModData = AttributeModData(this.Source, this.Source);
            var attributeModEntity = EntityManager.CreateEntity(
                                            typeof(ManaAttributeModifier),
                                            typeof(PermanentAttributeModification),
                                            typeof(AttributeModificationComponent),
                                            typeof(GameplayEffectDurationComponent)
            );

            EntityManager.SetComponentData(attributeModEntity, attributeModData);
        }
        public AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes) {
            attributes.Mana.CurrentValue -= ManaCost;
            return attributes;
        }

        private AttributeModificationComponent AttributeModData(Entity Source, Entity Target) {
            return new AttributeModificationComponent()
            {
                Add = -ManaCost,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };
        }
    }
}