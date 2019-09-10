using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.Fire {
    public struct FireAbilityComponent : IAbilityBehaviour, IComponentData {

        public EAbility AbilityType { get => EAbility.FireAbility; }

        public EGameplayEffect[] CooldownEffects => new EGameplayEffect[] { EGameplayEffect.GlobalCooldown, EGameplayEffect.FireAbilityCooldown };

        public IAbilityJobs AbilityJobs => new DefaultAbilityJobs<FireAbilityComponent>();

        public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new DefaultGameplayEffect().ApplyGameplayEffect<FireAbilityCost>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        }
        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            new DefaultGameplayEffect().ApplyGameplayEffect<FireAbilityCooldownEffect, GlobalCooldownEffect>(index, Ecb, Caster, Caster, new AttributesComponent(), WorldTime);
        }
        public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new DefaultGameplayEffect().ApplyGameplayEffect<FireGameplayEffect>(index, Ecb, Source, Target, new AttributesComponent(), WorldTime);
        }
        public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime) {
            new FireGameplayEffect() { Source = Source, Target = Target }.ApplyGameplayEffect(entityManager, attributesComponent, WorldTime);
        }
        public bool CheckResourceAvailable(ref Entity Caster, ref AttributesComponent attributes) {
            attributes = new FireAbilityCost().ComputeResourceUsage(Caster, attributes);
            return attributes.Mana.CurrentValue >= 0;
        }
    }
}

public struct DefaultGameplayEffect {

    public void ApplyGameplayEffect<T0>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect {
        var effect = new T0();
        effect.Source = Source;
        effect.Target = Target;
        effect.ApplyGameplayEffect(index, Ecb, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2, T3>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect
    where T3 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T3>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2, T3, T4>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect
    where T3 : struct, IComponentData, IGameplayEffect
    where T4 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T3>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T4>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2, T3, T4, T5>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect
    where T3 : struct, IComponentData, IGameplayEffect
    where T4 : struct, IComponentData, IGameplayEffect
    where T5 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T3>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T4>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T5>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

}