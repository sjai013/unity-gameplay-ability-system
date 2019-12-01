using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class HealthAttributeBarUpdateSystem : AttributeBarUpdateSystem<HealthAttributeComponent, MaxHealthAttributeComponent> {
    protected override UIAttributeUpdater GetAttributeBar(AttributeBarUpdater attributeBarUpdater) {
        return attributeBarUpdater.HP;
    }
}

public class ManaAttributeBarUpdateSystem : AttributeBarUpdateSystem<ManaAttributeComponent, MaxManaAttributeComponent> {
    protected override UIAttributeUpdater GetAttributeBar(AttributeBarUpdater attributeBarUpdater) {
        return attributeBarUpdater.Mana;
    }
}

public abstract class AttributeBarUpdateSystem<T1, T2> : ComponentSystem
where T1 : struct, IAttributeComponent, IComponentData
where T2 : struct, IAttributeComponent, IComponentData {

    protected abstract UIAttributeUpdater GetAttributeBar(AttributeBarUpdater attributeBarUpdater);
    protected override void OnUpdate() {
        // Identify all entities that need to be updated
        ComponentDataFromEntity<T1> attributeData = GetComponentDataFromEntity<T1>(true);
        ComponentDataFromEntity<T2> maxAttributeData = GetComponentDataFromEntity<T2>(true);
        Entities.WithAll<AttributeBarUpdater, ActorAbilitySystem>().ForEach((AttributeBarUpdater attributeBarUpdater, ActorAbilitySystem actorAbilitySystem) => {
            var attribute = attributeData[actorAbilitySystem.AbilityOwnerEntity];
            var maxAttribute = maxAttributeData[actorAbilitySystem.AbilityOwnerEntity];
            var attributeBar = GetAttributeBar(attributeBarUpdater);
            if (attributeBar == null) return;
            attributeBar.UpdateAttributeValue(attribute.CurrentValue, maxAttribute.CurrentValue);
        });
    }
}