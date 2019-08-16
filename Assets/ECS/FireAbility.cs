using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class FireAbilitySystem : AbilitySystem<FireAbility, FireAbilityCooldownJob> {

    protected override void OnCreate() {
        // cooldownQueryDesc = new EntityQueryDesc
        // {
        //     Any = new ComponentType[] { ComponentType.ReadOnly<FireCooldownTagComponent>(), ComponentType.ReadOnly<GlobalCooldownTagComponent>() }
        // };
        base.OnCreate();

    }
}

public struct FireAbilityCooldown : ICooldownSystemComponentDefinition {
    public EntityQueryDesc CooldownQueryDesc {
        get =>
                new EntityQueryDesc
                {
                    Any = new ComponentType[] { ComponentType.ReadOnly<FireCooldownTagComponent>(), ComponentType.ReadOnly<GlobalCooldownTagComponent>() },
                    All = new ComponentType[] { ComponentType.ReadOnly<CooldownEffectComponent>(), ComponentType.ReadOnly<GameplayEffectDurationComponent>() }
                };
    }
}

public class FireAbilityCooldownSystem : AbilityCooldownSystem<FireAbilityCooldown> { }
public struct FireAbilityCooldownJob : IJobForEachWithEntity<CooldownEffectComponent, GameplayEffectDurationComponent> {
    public void Execute(Entity entity, int index, ref CooldownEffectComponent c0, ref GameplayEffectDurationComponent c1) {
    }
}

public struct FireAbility : IAbility, IComponentData {
    public Entity Target { get; set; }
    public Entity Source { get; set; }

    public void ApplyAbilityCosts(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        new FireAbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
        new FireAbilityCost().ApplyGameplayEffect(index, Ecb, Source, Target, attributesComponent);
    }

    public void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime) {
        new FireAbilityCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
        new GlobalCooldownEffect().ApplyCooldownEffect(index, Ecb, Caster, WorldTime);
    }

    public void ApplyGameplayEffects(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {

    }

    public bool CheckResourceAvailable(Entity Caster, AttributesComponent attributes) {
        attributes = new FireAbilityCost().ComputeResourceUsage(Caster, attributes);
        attributes = new FireAbilityCost().ComputeResourceUsage(Caster, attributes);
        return attributes.Mana.CurrentValue >= 0;
    }
    public void CheckCooldownForCaster(NativeArray<GameplayEffectDurationComponent> CooldownEffects) {

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

public struct FireAbilityCost : ICost, IComponentData {
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