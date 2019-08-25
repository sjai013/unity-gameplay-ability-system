using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Heal {
    public struct HealCooldownTagComponent : IComponentData { }

    [BurstCompile]
    [RequireComponentTag(typeof(HealAbilityComponent))]
    public struct HealAbilityCooldownJob : ICooldownJob, IJobForEachWithEntity<AbilityCooldownComponent, AbilitySourceTarget>, ICooldownSystemComponentDefinition {
        public NativeArray<CooldownTimeCaster> CooldownArray { get => _cooldownArray; set => _cooldownArray = value; }

        [ReadOnly] private NativeArray<CooldownTimeCaster> _cooldownArray;
        public void Execute(Entity entity, int index, ref AbilityCooldownComponent cooldown, [ReadOnly] ref AbilitySourceTarget sourceTarget) {
            // Get the highest time remaining where the caster == entity
            var maxTimeRemaining = -1f;
            var duration = 0f;
            for (int i = 0; i < _cooldownArray.Length; i++) {
                if (sourceTarget.Source == _cooldownArray[i].Caster &&
                    _cooldownArray[i].TimeRemaining > maxTimeRemaining) {
                    maxTimeRemaining = _cooldownArray[i].TimeRemaining;
                    duration = _cooldownArray[i].Duration;
                }
            }
            cooldown.TimeRemaining = maxTimeRemaining;
            cooldown.Duration = duration;
        }

        public EntityQueryDesc CooldownQueryDesc {
            get =>
                    new EntityQueryDesc
                    {
                        Any = new ComponentType[] { ComponentType.ReadOnly<HealCooldownTagComponent>(), ComponentType.ReadOnly<GlobalCooldownTagComponent>() },
                        All = new ComponentType[] { ComponentType.ReadOnly<CooldownEffectComponent>(), ComponentType.ReadOnly<GameplayEffectDurationComponent>() }
                    };
        }
    }
}