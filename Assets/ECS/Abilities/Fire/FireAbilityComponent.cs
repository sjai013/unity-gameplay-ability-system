using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityComponent : IAbilityBehaviour, IComponentData{
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
            new FireGameplayEffect().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }
        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new FireGameplayEffect().ApplyGameplayEffect(entityManager, Source, Target, attributesComponent);
        }
        public bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes) {
            attributes = new FireAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }

        public JobHandle CooldownJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, float WorldTime) {
            var job = new GenericGatherCooldownsJob<FireAbilityComponent>()
            {
                Ecb = Ecb,
                WorldTime = WorldTime
            };
            return job.Schedule(system, inputDeps);
        }

        public JobHandle CostJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent) {

            var job = new GenericAbilityCostJob<FireAbilityComponent>()
            {
                Ecb = Ecb,
                attributesComponent = attributesComponent
            };

            return job.Schedule(system, inputDeps);

        }

    }

}

