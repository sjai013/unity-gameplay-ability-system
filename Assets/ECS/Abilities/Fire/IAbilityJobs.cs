using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public interface IAbilityJobs {
    JobHandle BeginAbilityCastJob(EntityQuery query, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime);
    JobHandle CheckAbilityAvailableJob(EntityQuery UpdateAbilityAvailable, EntityQuery CheckResource, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent, NativeHashMap<Entity, _GrantedAbilityCooldownComponent> abilityCooldowns);
    JobHandle CheckAbilityGrantedJob(EntityQuery query, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted);
    JobHandle UpdateCooldownsJob(EntityQuery query, JobHandle inputDeps, NativeHashMap<Entity, _GrantedAbilityCooldownComponent> cooldownsRemainingForAbility);
}