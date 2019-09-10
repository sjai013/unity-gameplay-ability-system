using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public struct DefaultAbilityJobs<T1> : IAbilityJobs
where T1 : struct, IComponentData, IAbilityBehaviour {
    public JobHandle BeginAbilityCastJob(JobComponentSystem system, JobHandle inputDeps, EntityCommandBuffer.Concurrent Ecb, ComponentDataFromEntity<AttributesComponent> attributesComponent, float WorldTime) {
        var job = new GenericBeginAbilityCast<T1>()
        {
            Ecb = Ecb,
            attributesComponent = attributesComponent,
            WorldTime = WorldTime
        };
        return job.Schedule(system, inputDeps);
    }

    public JobHandle UpdateCooldownsJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, GrantedAbilityCooldownComponent> cooldownsRemainingForAbility) {
        var job = new GenericUpdateAbilityCooldownJob<T1>()
        {
            cooldownsRemainingForAbility = cooldownsRemainingForAbility
        }.Schedule(system, inputDeps);

        return job;
    }

    public JobHandle CheckAbilityAvailableJob(JobComponentSystem system, JobHandle inputDeps, ComponentDataFromEntity<AttributesComponent> attributesComponent, NativeHashMap<Entity, GrantedAbilityCooldownComponent> abilityCooldowns) {
        var job1 = new GenericUpdateAbilityAvailableJob<T1>
        {
            cooldownsRemainingForAbility = abilityCooldowns
        }.Schedule(system, inputDeps);

        var job2 = new GenericCheckResourceForAbilityJob<T1>
        {
            attributesComponent = attributesComponent,
        }.Schedule(system, job1);
        return job2;
    }

    public JobHandle CheckAbilityGrantedJob(JobComponentSystem system, JobHandle inputDeps, NativeHashMap<Entity, bool> AbilityGranted) {
        var job = new GenericCheckAbilityGrantedJob<T1>
        {
            AbilityGranted = AbilityGranted.AsParallelWriter()
        };
        return job.Schedule(system, inputDeps);
    }
}
