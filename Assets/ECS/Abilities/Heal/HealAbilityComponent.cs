using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealAbilityComponent : IAbilityBehaviour, IComponentData {
        public EAbility AbilityType { get => EAbility.HealAbility; }
        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.HealAbilityCooldown };
        public IAbilityJobs AbilityJobs => new DefaultAbilityJobs<HealAbilityComponent>();

        public IEntityQueryDescContainer EntityQueries => new EntityQueryDescContainerBasic<HealAbilityComponent>();

        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new DefaultGameplayEffect().ApplyGameplayEffect<HealAbilityCost>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        }
        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            new DefaultGameplayEffect().ApplyGameplayEffect<HealAbilityCooldownEffect, GlobalCooldownEffect>(index, Ecb, Caster, Caster, new AttributesComponent(), WorldTime);
        }
        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new DefaultGameplayEffect().ApplyGameplayEffect<HealGameplayEffect>(index, Ecb, Source, Target, attributesComponent, WorldTime);
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