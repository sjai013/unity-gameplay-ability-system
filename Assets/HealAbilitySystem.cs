using GameplayAbilitySystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class HealAbilitySystem : AbilitySystem<HealAbilityComponent, HealAbilityCooldownJob> {
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

        public void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
            var attributeModData = new AttributeModificationComponent()
            {
                Add = -ManaCost,
                Multiply = 0,
                Divide = 0,
                Change = 0,
                Source = Source,
                Target = Target
            };

            var attributeModEntity = EntityManager.CreateEntity(
                                            typeof(ManaAttributeModifier),
                                            typeof(PermanentAttributeModification),
                                            typeof(AttributeModificationComponent)
            );
            EntityManager.SetComponentData(attributeModEntity, attributeModData);
        }

        public AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes) {
            attributes.Mana.CurrentValue -= ManaCost;
            return attributes;
        }
    }
}

public struct HealAbilityComponent : IAbility, IComponentData {
    public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new HealAbilitySystem.AbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
    }

    public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
        new AbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
    }

    public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new HealGameplayEffect().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
    }

    public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new HealGameplayEffect().ApplyGameplayEffect(entityManager, Source, Target, attributesComponent);
    }

    public bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes) {
        attributes = new HealAbilitySystem.AbilityCost().ComputeResourceUsage(Caster, attributes);
        return attributes.Mana.CurrentValue >= 0;
    }

    public struct AbilityCooldownEffect : ICooldown, IComponentData {
        const float Duration = 3f;
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
            Ecb.AddComponent(index, attributeModEntity, new HealCooldownTagComponent());
        }
    }
}

public struct HealGameplayEffect : IGameplayEffect, IComponentData {
    public Entity Target { get; set; }
    public Entity Source { get; set; }
    const float DamageAdder = +15;
    public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        var attributeModData = AttributeModData();
        var attributeModEntity = Ecb.CreateEntity(index);
        Ecb.AddComponent(index, attributeModEntity, new HealthAttributeModifier());
        Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
        Ecb.AddComponent(index, attributeModEntity, attributeModData);
    }

    public void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        var attributeModData = AttributeModData();
        var attributeModEntity = EntityManager.CreateEntity(
                                                    typeof(HealthAttributeModifier),
                                                    typeof(PermanentAttributeModification),
                                                    typeof(AttributeModificationComponent)
        );
        EntityManager.SetComponentData(attributeModEntity, attributeModData);
    }
    private AttributeModificationComponent AttributeModData() {
        return new AttributeModificationComponent()
        {
            Add = DamageAdder,
            Multiply = 0,
            Divide = 0,
            Change = 0,
            Source = Source,
            Target = Target
        };
    }

}

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

public class HealAbilityActivationSystem : AbilityActivationSystem<HealAbilityComponent> {
    public GameObject Prefab;

    protected override void OnUpdate() {
        // This is a simple system, where there is only one projectile to worry about.
        // We could create a new entity to capture the position of the projectile, but to keep it simple
        // we use the existing entity

        Entities.WithAll<HealAbilityComponent, AbilityStateComponent, AbilitySourceTarget>().ForEach((Entity entity, ref HealAbilityComponent ability, ref AbilityStateComponent abilityState, ref AbilitySourceTarget sourceTarget) => {
            if (abilityState.State != EAbilityState.Activate) return;
            var transforms = GetComponentDataFromEntity<LocalToWorld>(true);
            var sourceTransform = transforms[sourceTarget.Source];
            var sourceAbilitySystem = EntityManager.GetComponentObject<AbilitySystemComponent>(sourceTarget.Source);
            var targetAbilitySystem = EntityManager.GetComponentObject<AbilitySystemComponent>(sourceTarget.Target);
            ActivateAbility(sourceAbilitySystem, targetAbilitySystem, entity, ability);
            abilityState.State = EAbilityState.Completed;
        });
    }

    private void ActivateAbility(AbilitySystemComponent Source, AbilitySystemComponent Target, Entity AbilityEntity, HealAbilityComponent ability) {
        var abilitySystemActor = Source.GetActor();


        //(_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
        var gameplayEventData = new GameplayAbilitySystem.Events.GameplayEventData()
        {
            Instigator = Source,
            Target = Target
        };

        GameObject instantiatedProjectile = null;

        instantiatedProjectile = MonoBehaviour.Instantiate(Prefab);
        instantiatedProjectile.transform.position = abilitySystemActor.transform.position + new Vector3(0, 1.5f, 0) + abilitySystemActor.transform.forward * 1.2f;

        // Animation complete.  Spawn and send projectile at target
        if (instantiatedProjectile != null) {
            SeekTargetAndDestroy(Source, Target, instantiatedProjectile, ability, AbilityEntity);
        }


    }

    private async void SeekTargetAndDestroy(AbilitySystemComponent Source, AbilitySystemComponent Target, GameObject projectile, HealAbilityComponent Ability, Entity AbilityEntity) {
        await projectile.GetComponent<Projectile>().SeekTarget(Target.TargettingLocation.gameObject, Target.gameObject);
        var attributesComponent = GetComponentDataFromEntity<AttributesComponent>(false);
        Ability.ApplyGameplayEffects(World.Active.EntityManager, Source.entity, Target.entity, attributesComponent[Target.entity]);
        MonoBehaviour.DestroyImmediate(projectile);

    }
}