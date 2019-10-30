using Unity.Entities;

namespace GameplayAbilitySystem.GameplayEffects.Systems {
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public class GameplayEffectGroupUpdateBeginSystem : ComponentSystemGroup { }

    [UpdateAfter(typeof(GameplayEffectGroupUpdateBeginSystem))]
    public class GameplayEffectGroupUpdateEndSystem : ComponentSystemGroup { }
}