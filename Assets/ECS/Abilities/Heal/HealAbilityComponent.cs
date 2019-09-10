using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealAbilityComponent : IAbilityBehaviour, IComponentData {

        public EAbility AbilityType { get => EAbility.HealAbility; }
        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.HealAbilityCooldown };

        public IAbilityJobs AbilityJobs => new DefaultAbilityJobs<HealAbilityComponent>();

        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new HealAbilityCost().ApplyGameplayEffect(index, Ecb, attributesComponent, WorldTime);
        }

        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            new HealAbilityCooldownEffect() { Source = Caster }.ApplyGameplayEffect(index, Ecb, new AttributesComponent(), WorldTime);
            new GlobalCooldownEffect() { Source = Caster }.ApplyGameplayEffect(index, Ecb, new AttributesComponent(), WorldTime);
        }

        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new HealGameplayEffect() { Source = Source, Target = Target }.ApplyGameplayEffect(index, Ecb, attributesComponent, WorldTime);
        }

        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new HealGameplayEffect() { Source = Source, Target = Target }.ApplyGameplayEffect(entityManager, attributesComponent, WorldTime);
        }
        public bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes) {
            attributes = new HealAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }
    }
}