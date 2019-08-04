using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

public class HealthAttributeModificationSystem : GameplayEffectAttributeModificationSystem<HealthAttributeModifierJob> {

}

public class HealthAttributeModificationUndoSystem : GameplayEffectAttributeModificationUndoSystem<HealthAttributeUndoJob> {

}

public class ManaAttributeModificationSystem : GameplayEffectAttributeModificationSystem<ManaAttributeModifierJob> {

}

public class ManaAttributeModificationUndoSystem : GameplayEffectAttributeModificationUndoSystem<ManaAttributeUndoJob> {

}

[RequireComponentTag(typeof(AttributeModifyComponent), typeof(HealthAttributeModifier))]
public struct HealthAttributeModifierJob : AttributeModifierJob {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    public ComponentDataFromEntity<AttributesComponent> attrComponents { get => _attrComponents; set => _attrComponents = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            attrs.Health.CurrentValue += attrMod.Change; ;
            _attrComponents[attrMod.Target] = attrs;
        }

        Ecb.RemoveComponent<AttributeModifyComponent>(index, entity);
    }
}

[BurstCompile]
[RequireComponentTag(typeof(GameplayEffectExpired), typeof(TemporaryAttributeModification), typeof(HealthAttributeModifier))]
public struct HealthAttributeUndoJob : AttributeModifierUndoJob {
    [NativeDisableContainerSafetyRestriction] [WriteOnly] public ComponentDataFromEntity<AttributesComponent> _attrComponents;
    public ComponentDataFromEntity<AttributesComponent> attrComponents { get => _attrComponents; set => _attrComponents = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            attrs.Health.CurrentValue -= attrMod.Change; ;
            _attrComponents[attrMod.Target] = attrs;
        }
    }
}


[RequireComponentTag(typeof(AttributeModifyComponent), typeof(ManaAttributeModifier))]
public struct ManaAttributeModifierJob : AttributeModifierJob {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    public ComponentDataFromEntity<AttributesComponent> attrComponents { get => _attrComponents; set => _attrComponents = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            attrs.Mana.CurrentValue += attrMod.Change; ;
            _attrComponents[attrMod.Target] = attrs;
        }

        Ecb.RemoveComponent<AttributeModifyComponent>(index, entity);
    }
}


[BurstCompile]
[RequireComponentTag(typeof(GameplayEffectExpired), typeof(TemporaryAttributeModification), typeof(ManaAttributeModifier))]
//[ExcludeComponent(typeof(AttributeModificationUndoAppliedComponent))]
public struct ManaAttributeUndoJob : AttributeModifierUndoJob {
    [NativeDisableContainerSafetyRestriction] [WriteOnly] public ComponentDataFromEntity<AttributesComponent> _attrComponents;
    public ComponentDataFromEntity<AttributesComponent> attrComponents { get => _attrComponents; set => _attrComponents = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            attrs.Mana.CurrentValue -= attrMod.Change; ;
            _attrComponents[attrMod.Target] = attrs;
        }
    }
}