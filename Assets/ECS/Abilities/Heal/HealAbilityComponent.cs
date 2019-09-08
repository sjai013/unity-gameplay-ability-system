using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Heal {
    public partial struct HealAbilityComponent : IAbilityBehaviour, IComponentData {

        public EAbility AbilityType { get => EAbility.HealAbility; }
        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.HealAbilityCooldown };

        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new HealAbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }

        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            new HealAbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
            new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        }

        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new HealGameplayEffect().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        }

        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            new HealGameplayEffect().ApplyGameplayEffect(entityManager, Source, Target, attributesComponent);
        }

        public JobHandle CheckAbilityAvailableJob(JobComponentSystem system, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent) {
            var job = new GenericCheckResourceForAbilityJob<HealAbilityComponent>
            {
                attributesComponent = attributesComponent,
            };
            return job.Schedule(system, inputDeps);
        }

        public bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes) {
            attributes = new HealAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }

        public JobHandle BeginAbilityCastJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime) {
            var job = new GenericBeginAbilityCast<HealAbilityComponent>()
            {
                Ecb = Ecb,
                attributesComponent = attributesComponent,
                WorldTime = WorldTime
            };
            return job.Schedule(system, inputDeps);
        }
        public JobHandle UpdateCooldownsJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility) {
            var job = new GenericUpdateAbilityCooldownJob<HealAbilityComponent>()
            {
                cooldownsRemainingForAbility = cooldownsRemainingForAbility
            };
            return job.Schedule(system, inputDeps);
        }

        public JobHandle CheckAbilityGrantedJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted) {
            var job = new GenericCheckAbilityGrantedJob<HealAbilityComponent>
            {
                AbilityGranted = AbilityGranted.AsParallelWriter()
            };
            return job.Schedule(system, inputDeps);
        }
    }
}