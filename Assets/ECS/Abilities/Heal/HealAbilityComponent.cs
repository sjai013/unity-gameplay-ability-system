using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealAbilityComponent : IAbilityBehaviour, IComponentData {

        public EAbility AbilityType { get => EAbility.HealAbility; }
        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.HealAbilityCooldown };

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
        public JobHandle CheckAbilityAvailableJob(JobComponentSystem system, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent, NativeHashMap<Entity, GrantedAbilityCooldownComponent> abilityCooldowns) {
            var job1 = new GenericUpdateAbilityAvailableJob<HealAbilityComponent>
            {
                cooldownsRemainingForAbility = abilityCooldowns
            }.Schedule(system, inputDeps);

            var job2 = new GenericCheckResourceForAbilityJob<HealAbilityComponent>
            {
                attributesComponent = attributesComponent,
            }.Schedule(system, job1);
            return job2;
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