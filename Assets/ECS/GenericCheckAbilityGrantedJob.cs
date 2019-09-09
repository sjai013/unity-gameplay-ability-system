using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[RequireComponentTag(typeof(GrantedAbilityComponent), typeof(GrantedAbilityCooldownComponent))]
public struct GenericCheckAbilityGrantedJob<T1> : IJobForEachWithEntity<T1>
where T1 : struct, IComponentData, IAbilityBehaviour {

    [WriteOnly] public NativeHashMap<Entity, bool>.ParallelWriter AbilityGranted;

    public void Execute(Entity entity, int index, [ReadOnly] ref T1 Ability) {
        AbilityGranted.TryAdd(entity, true);
    }
}
