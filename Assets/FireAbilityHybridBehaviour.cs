using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities.AbilityActivations;
using GameplayAbilitySystem.ExtensionMethods;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FireAbilityHybridBehaviour : MonoBehaviour, IConvertGameObjectToEntity {
    public RangeAttack attackBehaviour;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {

    }
}

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

public struct FireAbility : IAbility, IComponentData {
    public Entity Target { get => _target; set => _target = value; }
    public Entity _target;
    public Entity Source { get => _source; set => _source = value; }
    public Entity _source;
    public float CooldownTimeRemaining { get => _cooldownTimeRemaining; set => _cooldownTimeRemaining = value; }
    public float _cooldownTimeRemaining;

    public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new FireAbilitySystem.AbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
    }

    public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
        new FireAbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
    }

    public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new FireGameplayEffect().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
    }

    public void ApplyGameplayEffects(EntityManager entityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new FireGameplayEffect().ApplyGameplayEffect(entityManager, Source, Target, attributesComponent);
    }

    public bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes) {
        attributes = new FireAbilitySystem.AbilityCost().ComputeResourceUsage(Caster, attributes);
        attributes = new FireAbilitySystem.AbilityCost().ComputeResourceUsage(Caster, attributes);
        return attributes.Mana.CurrentValue >= 0;
    }

    public struct FireAbilityCooldownEffect : ICooldown, IComponentData {
        const float Duration = 5f;
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

    public void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        var attributeModData = new AttributeModificationComponent()
        {
            Add = DamageAdder,
            Multiply = DamageMultiplier,
            Divide = 0,
            Change = 0,
            Source = Source,
            Target = Target
        };

        var attributeModEntity = EntityManager.CreateEntity(
                                                    typeof(HealthAttributeModifier),
                                                    typeof(PermanentAttributeModification),
                                                    typeof(AttributeModificationComponent)
        );
        EntityManager.SetComponentData(attributeModEntity, attributeModData);
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

public class FireAbilityActivationSystem : AbilityActivationSystem<FireAbility, FireAbilityHybridBehaviour> {
    private const string ProjectileFireTriggerName = "Execute_Magic";
    private const string CastAnimationTriggerName = "Do_Magic";
    private const string CompletionAnimatorStateFull = "Base.Idle";
    public GameplayAbilitySystem.AnimationEvent CastingStartEvent;
    public GameplayAbilitySystem.AnimationEvent FireProjectileEvent;
    public GameObject Prefab;

    protected override void OnUpdate() {
        // This is a simple system, where there is only one projectile to worry about.
        // We could create a new entity to capture the position of the projectile, but to keep it simple
        // we use the existing entity

        Entities.WithAll<FireAbility, ActivateAbilityComponent>().ForEach((Entity entity, ref FireAbility ability) => {
            var transforms = GetComponentDataFromEntity<LocalToWorld>(true);
            var sourceTransform = transforms[ability.Source];
            var source = EntityManager.GetComponentObject<AbilitySystemComponent>(ability.Source);
            var target = EntityManager.GetComponentObject<AbilitySystemComponent>(ability.Target);
            ActivateAbility(source, target, entity, ability);
            World.Active.EntityManager.RemoveComponent<ActivateAbilityComponent>(entity);
            World.Active.EntityManager.AddComponent<AbilityActiveComponent>(entity);
        });
    }

    private async void ActivateAbility(AbilitySystemComponent Source, AbilitySystemComponent Target, Entity AbilityEntity, FireAbility ability) {
        var abilitySystemActor = Source.GetActor();
        var animationEventSystemComponent = abilitySystemActor.GetComponent<AnimationEventSystem>();
        var animatorComponent = abilitySystemActor.GetComponent<Animator>();
        World.Active.EntityManager.RemoveComponent<AbilityActiveComponent>(AbilityEntity);
        World.Active.EntityManager.AddComponent<AbilityActivatedComponent>(AbilityEntity);

        // Make sure we have enough resources.  End ability if we don't

        animatorComponent.SetTrigger(CastAnimationTriggerName);
        //(_, var gameplayEventData) = await AbilitySystem.OnGameplayEvent.WaitForEvent((gameplayTag, eventData) => gameplayTag == WaitForEventTag);
        var gameplayEventData = new GameplayAbilitySystem.Events.GameplayEventData()
        {
            Instigator = Source,
            Target = Target
        };

        GameObject instantiatedProjectile = null;

        await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == CastingStartEvent);

        instantiatedProjectile = MonoBehaviour.Instantiate(Prefab);
        instantiatedProjectile.transform.position = abilitySystemActor.transform.position + new Vector3(0, 1.5f, 0) + abilitySystemActor.transform.forward * 1.2f;

        animatorComponent.SetTrigger(ProjectileFireTriggerName);

        await animationEventSystemComponent.CustomAnimationEvent.WaitForEvent((x) => x == FireProjectileEvent);

        // Animation complete.  Spawn and send projectile at target
        if (instantiatedProjectile != null) {
            SeekTargetAndDestroy(Source, Target, instantiatedProjectile, ability);
        }


        var beh = animatorComponent.GetBehaviour<AnimationBehaviourEventSystem>();
        await beh.StateEnter.WaitForEvent((animator, stateInfo, layerIndex) => stateInfo.fullPathHash == Animator.StringToHash(CompletionAnimatorStateFull));
    }

    private async void SeekTargetAndDestroy(AbilitySystemComponent Source, AbilitySystemComponent Target, GameObject projectile, FireAbility Ability) {
        await projectile.GetComponent<Projectile>().SeekTarget(Target.TargettingLocation.gameObject, Target.gameObject);
        var attributesComponent = GetComponentDataFromEntity<AttributesComponent>(false);
        Ability.ApplyGameplayEffects(World.Active.EntityManager, Source.entity, Target.entity, attributesComponent[Target.entity]);
        MonoBehaviour.DestroyImmediate(projectile);

    }
}

public struct FireAbilityTagComponent : IComponentData { }