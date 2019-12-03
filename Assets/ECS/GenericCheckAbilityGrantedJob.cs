using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[RequireComponentTag(typeof(GrantedAbilityComponent), typeof(_GrantedAbilityCooldownComponent))]
public struct GenericCheckAbilityGrantedJob<T1> : IJobForEachWithEntity<T1>
where T1 : struct, IComponentData, _IAbilityBehaviour {

    [WriteOnly] public NativeHashMap<Entity, bool>.ParallelWriter AbilityGranted;

    public void Execute(Entity entity, int index, [ReadOnly] ref T1 Ability) {
        AbilityGranted.TryAdd(entity, true);
    }
}
