using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealGameplayEffect : IGameplayEffect, IComponentData {
        public DurationPolicyComponent DurationPolicy { get; set; }
        public Entity Target { get; set; }
        public Entity Source { get; set; }
        const float DamageAdder = +15;
        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime) {
            var attributeModData = AttributeModData();
            // var gameplayEffectData = new GameplayEffectDurationComponent()
            // {
            //     WorldStartTime = WorldTime,
            //     Duration = 5,
            //     Effect = EGameplayEffect.HealRegen
            // };

            var attributeModEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, attributeModEntity, new HealthAttributeModifier());
            // Ecb.AddComponent(index, attributeModEntity, new TemporaryAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
            // Ecb.AddComponent(index, attributeModEntity, gameplayEffectData);

            Ecb.AddComponent(index, attributeModEntity, attributeModData);

    
        }

        public void ApplyGameplayEffect(EntityManager EntityManager, AttributesComponent attributesComponent, float WorldTime) {
            var attributeModData = AttributeModData();
            var attributeModEntity = EntityManager.CreateEntity(
                                                        typeof(HealthAttributeModifier),
                                                        // typeof(TemporaryAttributeModification),
                                                        typeof(PermanentAttributeModification),
                                                        typeof(AttributeModificationComponent),
                                                        typeof(GameplayEffectDurationComponent)
            );


            // var gameplayEffectData = new GameplayEffectDurationComponent()
            // {
            //     WorldStartTime = WorldTime,
            //     Duration = 10,
            //     Effect = EGameplayEffect.HealRegen
            // };


            EntityManager.SetComponentData(attributeModEntity, attributeModData);
            // EntityManager.SetComponentData(attributeModEntity, gameplayEffectData);
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

