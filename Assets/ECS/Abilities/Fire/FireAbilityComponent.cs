using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityComponent : IAbilityBehaviour, IComponentData {

        public EAbility AbilityType { get => EAbility.FireAbility; }

        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.FireAbilityCooldown };

        public IAbilityJobs AbilityJobs => new DefaultAbilityJobs<FireAbilityComponent>();

        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new FireAbilityCost() { Source = Source, Target = Target }.ApplyGameplayEffect(index, Ecb, attributesComponent, WorldTime);
        }

        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            // new FireAbilityCooldownEffect() { Source = Caster }.ApplyGameplayEffect(index, Ecb, new AttributesComponent(), WorldTime);
            new FireAbilityCooldownEffect() { Source = Caster }.ApplyGameplayEffect(index, Ecb, new AttributesComponent(), WorldTime);
            new GlobalCooldownEffect() { Source = Caster }.ApplyGameplayEffect(index, Ecb, new AttributesComponent(), WorldTime);
        }
        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new FireGameplayEffect() { Source = Source, Target = Target }.ApplyGameplayEffect(index, Ecb, attributesComponent, WorldTime);
        }
        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new FireGameplayEffect() { Source = Source, Target = Target }.ApplyGameplayEffect(entityManager, attributesComponent, WorldTime);
        }
        public bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes) {
            attributes = new FireAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }


    }

}
