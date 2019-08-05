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
    [ReadOnly] private ComponentDataFromEntity<TemporaryAttributeModification> _tempAttributeModifier;
    [ReadOnly] private ComponentDataFromEntity<PermanentAttributeModification> _permanentAttributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<TemporaryAttributeModification> TempAttributeModifier { get => _tempAttributeModifier; set => _tempAttributeModifier = value; }
    public ComponentDataFromEntity<PermanentAttributeModification> PermanentAttributeModifier { get => _permanentAttributeModifier; set => _permanentAttributeModifier = value; }

    public void Execute(Entity entity, int index, ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Health;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);

            if (_tempAttributeModifier.Exists(entity)) {
                attr.CurrentValue += attrMod.Change;
            }
            if (_permanentAttributeModifier.Exists(entity)) {
                attr.BaseValue += attrMod.Change;
            }
            attrs.Health = attr;
            _attrComponents[attrMod.Target] = attrs;
        }

        if (_permanentAttributeModifier.Exists(entity)) {
            Ecb.DestroyEntity(index, entity);
        } else {
            Ecb.RemoveComponent<AttributeModifyComponent>(index, entity);
        }
    }
}

[BurstCompile]
[RequireComponentTag(typeof(GameplayEffectExpired), typeof(TemporaryAttributeModification), typeof(HealthAttributeModifier))]
public struct HealthAttributeUndoJob : AttributeModifierUndoJob {
    [NativeDisableContainerSafetyRestriction] [WriteOnly] public ComponentDataFromEntity<AttributesComponent> _attrComponents;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }

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
    [ReadOnly] private ComponentDataFromEntity<TemporaryAttributeModification> _tempAttributeModifier;
    [ReadOnly] private ComponentDataFromEntity<PermanentAttributeModification> _permanentAttributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<TemporaryAttributeModification> TempAttributeModifier { get => _tempAttributeModifier; set => _tempAttributeModifier = value; }
    public ComponentDataFromEntity<PermanentAttributeModification> PermanentAttributeModifier { get => _permanentAttributeModifier; set => _permanentAttributeModifier = value; }
    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Mana;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);

            if (_tempAttributeModifier.Exists(entity)) {
                attr.CurrentValue += attrMod.Change;
            }
            if (_permanentAttributeModifier.Exists(entity)) {
                attr.CurrentValue += attrMod.Change;
                attr.BaseValue += attrMod.Change;
            }
            attrs.Mana = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
        if (_permanentAttributeModifier.Exists(entity)) {
            Ecb.DestroyEntity(index, entity);
        } else {
            Ecb.RemoveComponent<AttributeModifyComponent>(index, entity);
        }
    }
}


[BurstCompile]
[RequireComponentTag(typeof(GameplayEffectExpired), typeof(TemporaryAttributeModification), typeof(ManaAttributeModifier))]
//[ExcludeComponent(typeof(AttributeModificationUndoAppliedComponent))]
public struct ManaAttributeUndoJob : AttributeModifierUndoJob {
    [NativeDisableContainerSafetyRestriction] [WriteOnly] public ComponentDataFromEntity<AttributesComponent> _attrComponents;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            attrs.Mana.CurrentValue -= attrMod.Change; ;
            _attrComponents[attrMod.Target] = attrs;
        }
    }
}