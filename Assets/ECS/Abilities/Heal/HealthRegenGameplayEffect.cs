using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.HealthRegen {
    public struct HealthRegenGameplayEffect : _IPeriodicEffect, IComponentData {
        public DurationPolicyComponent DurationPolicy { get; set; }
        public Entity Target { get; set; }
        public Entity Source { get; set; }
        const float DamageAdder = +5;
        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime) {
            var attributeModData = AttributeModData();

            var attributeModEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, attributeModEntity, new _HealthAttributeModifier());
            // Ecb.AddComponent(index, attributeModEntity, new TemporaryAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
            // Ecb.AddComponent(index, attributeModEntity, gameplayEffectData);

            Ecb.AddComponent(index, attributeModEntity, attributeModData);
        }

        public void ApplyGameplayEffect(EntityManager EntityManager, AttributesComponent attributesComponent, float WorldTime) {
        }
        private _AttributeModificationComponent AttributeModData() {
            return new _AttributeModificationComponent()
            {
                Add = DamageAdder,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };
        }

        public void CreatePeriodicEffectEntity(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime) {
            throw new System.NotImplementedException();
        }
    }
}

