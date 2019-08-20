using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class FireAbilitySystem : AbilitySystem<FireAbility, FireAbilityCooldownJob> {
    public struct AbilityCost : ICost, IComponentData {
        const int ManaCost = 2;
        public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            var attributeModData = new AttributeModificationComponent()
            {
                Add = -ManaCost,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };

            var attributeModEntity = Ecb.CreateEntity(index);
            Ecb.AddComponent(index, attributeModEntity, new ManaAttributeModifier());
            Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, attributeModData);
        }
        public AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes) {
            attributes.Mana.CurrentValue -= ManaCost;
            return attributes;
        }
    }



}

public struct
FireAbility : IAbility, IComponentData {
    public Entity Target { get => _target; set => _target = value; }
    public Entity _target;
    public Entity Source { get => _source; set => _source = value; }
    public Entity _source;
    public float CooldownTimeRemaining { get => _cooldownTimeRemaining; set => _cooldownTimeRemaining = value; }
    public float _cooldownTimeRemaining;

    public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new FireAbilitySystem.AbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        new FireAbilitySystem.AbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
    }

    public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
        new FireAbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
    }

    public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {

    }

    public bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes) {
        attributes = new FireAbilitySystem.AbilityCost().ComputeResourceUsage(Caster, attributes);
        attributes = new FireAbilitySystem.AbilityCost().ComputeResourceUsage(Caster, attributes);
        return attributes.Mana.CurrentValue >= 0;
    }

    public struct FireAbilityCooldownEffect : ICooldown, IComponentData {
        const float Duration = 10;
        public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
            var attributeModData = new AttributeModificationComponent()
            {
                Add = 0,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Caster,
                Target = Caster
            };

            var attributeModEntity = Ecb.CreateEntity(index);
            var gameplayEffectData = new GameplayEffectDurationComponent()
            {
                WorldStartTime = WorldTime,
                Duration = Duration,
            };
            var cooldownEffectComponent = new CooldownEffectComponent()
            {
                Caster = Caster
            };

            Ecb.AddComponent(index, attributeModEntity, new NullAttributeModifier());
            Ecb.AddComponent(index, attributeModEntity, new TemporaryAttributeModification());
            Ecb.AddComponent(index, attributeModEntity, gameplayEffectData);
            Ecb.AddComponent(index, attributeModEntity, attributeModData);
            Ecb.AddComponent(index, attributeModEntity, cooldownEffectComponent);
            Ecb.AddComponent(index, attributeModEntity, new FireCooldownTagComponent());
        }
    }

}


public struct FireGameplayEffect : IGameplayEffect, IComponentData {
    public Entity Target { get; set; }
    public Entity Source { get; set; }
    const float DamageMultiplier = -0.1f;
    const float DamageAdder = -5;
    public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        var attributeModData = new AttributeModificationComponent()
        {
            Add = DamageAdder,
            Multiply = DamageMultiplier,
            Divide = 0,
            Change = 0,
            Source = Source,
            Target = Target
        };

        var attributeModEntity = Ecb.CreateEntity(index);
        Ecb.AddComponent(index, attributeModEntity, new HealthAttributeModifier());
        Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
        Ecb.AddComponent(index, attributeModEntity, attributeModData);
    }
}

public struct FireCooldownTagComponent : IComponentData { }

[BurstCompile]
public struct FireAbilityCooldownJob : ICooldownJob, IJobForEachWithEntity<FireAbility>, ICooldownSystemComponentDefinition {
    public NativeArray<CooldownTimeCaster> CooldownArray { get => _cooldownArray; set => _cooldownArray = value; }

    [ReadOnly] private NativeArray<CooldownTimeCaster> _cooldownArray;
    public void Execute(Entity entity, int index, [ReadOnly] ref FireAbility ability) {
        // Get the highest time remaining where the caster == entity
        var maxTimeRemaining = 0f;
        for (int i = 0; i < _cooldownArray.Length; i++) {
            if (ability.Source.Equals(_cooldownArray[i].Caster) &&
                _cooldownArray[i].TimeRemaining > maxTimeRemaining) {
                maxTimeRemaining = _cooldownArray[i].TimeRemaining;
            }
        }
        ability.CooldownTimeRemaining = maxTimeRemaining;
    }

    public EntityQueryDesc CooldownQueryDesc {
        get =>
                new EntityQueryDesc
                {
                    Any = new ComponentType[] { ComponentType.ReadOnly<FireCooldownTagComponent>(), ComponentType.ReadOnly<GlobalCooldownTagComponent>() },
                    All = new ComponentType[] { ComponentType.ReadOnly<CooldownEffectComponent>(), ComponentType.ReadOnly<GameplayEffectDurationComponent>() }
                };
    }
}