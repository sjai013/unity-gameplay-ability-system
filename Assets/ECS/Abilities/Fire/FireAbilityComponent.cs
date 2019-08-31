using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityComponent : IAbility, IComponentData {
        public EAbility AbilityType { get => EAbility.FireAbility; }

        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.FireAbilityCooldown };

        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new FireAbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }
        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            new FireAbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
            new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        }
        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            Debug.Log("Fire Ability");
            new FireGameplayEffect().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }
        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new FireGameplayEffect().ApplyGameplayEffect(entityManager, Source, Target, attributesComponent);
        }
        public bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes) {
            attributes = new FireAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }
    }
}