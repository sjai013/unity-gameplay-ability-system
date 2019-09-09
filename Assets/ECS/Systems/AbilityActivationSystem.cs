using Unity.Entities;

namespace GameplayAbilitySystem.Abilities {
    public abstract class AbilityActivationSystem<T1> : ComponentSystem
    where T1 : struct, IComponentData {
    }
}