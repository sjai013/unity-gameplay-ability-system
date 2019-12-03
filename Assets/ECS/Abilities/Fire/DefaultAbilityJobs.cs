using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public struct DefaultAbilityJobs<T1> : IAbilityJobs
where T1 : struct, IComponentData, _IAbilityBehaviour {
    public JobHandle BeginAbilityCastJob(EntityQuery query, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime) {
        var job = new GenericBeginAbilityCast<T1>()
        {
            Ecb = Ecb,
            attributesComponent = attributesComponent,
            WorldTime = WorldTime
        };
        return job.Schedule(query, inputDeps);
    }

    public JobHandle UpdateCooldownsJob(EntityQuery query, JobHandle inputDeps, NativeHashMap<Entity, _GrantedAbilityCooldownComponent> cooldownsRemainingForAbility) {
        var job = new GenericUpdateAbilityCooldownJob<T1>()
        {
            cooldownsRemainingForAbility = cooldownsRemainingForAbility
        }.Schedule(query, inputDeps);

        return job;
    }

    public JobHandle CheckAbilityAvailableJob(EntityQuery UpdateAbilityAvailable, EntityQuery CheckResource, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent, NativeHashMap<Entity, _GrantedAbilityCooldownComponent> abilityCooldowns) {
        var job1 = new _GenericUpdateAbilityAvailableJob<T1>
        {
            cooldownsRemainingForAbility = abilityCooldowns
        }.Schedule(UpdateAbilityAvailable, inputDeps);

        var job2 = new GenericCheckResourceForAbilityJob<T1>
        {
            attributesComponent = attributesComponent,
        }.Schedule(CheckResource, job1);
        return job2;
    }

    public JobHandle CheckAbilityGrantedJob(EntityQuery query, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted) {
        var job = new GenericCheckAbilityGrantedJob<T1>
        {
            AbilityGranted = AbilityGranted.AsParallelWriter()
        };
        return job.Schedule(query, inputDeps);
    }
}
