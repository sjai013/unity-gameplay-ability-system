using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityComponent : IAbilityBehaviour, IComponentData {
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

        public JobHandle BeginAbilityCastJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime) {
            var job = new GenericBeginAbilityCast<FireAbilityComponent>()
            {
                Ecb = Ecb,
                attributesComponent = attributesComponent,
                WorldTime = WorldTime
            };
            return job.Schedule(system, inputDeps);
        }

        public JobHandle UpdateCooldownsJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility) {
            var job = new GenericUpdateAbilityCooldownJob<FireAbilityComponent>()
            {
                cooldownsRemainingForAbility = cooldownsRemainingForAbility
            }.Schedule(system, inputDeps);

            return job;
        }

        public JobHandle CheckAbilityAvailableJob(JobComponentSystem system, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent, NativeHashMap<Entity, GrantedAbilityCooldownComponent> abilityCooldowns) {
            var job1 = new GenericUpdateAbilityAvailableJob<FireAbilityComponent>
            {
                cooldownsRemainingForAbility = abilityCooldowns
            }.Schedule(system, inputDeps);

            var job2 = new GenericCheckResourceForAbilityJob<FireAbilityComponent>
            {
                attributesComponent = attributesComponent,
            }.Schedule(system, job1);
            return job2;
        }

        public JobHandle CheckAbilityGrantedJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted) {
            var job = new GenericCheckAbilityGrantedJob<FireAbilityComponent>
            {
                AbilityGranted = AbilityGranted.AsParallelWriter()
            };
            return job.Schedule(system, inputDeps);
        }
    }

}
