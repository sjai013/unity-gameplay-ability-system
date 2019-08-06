using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[UpdateAfter(typeof(ResetAttributesDeltaSystem))]
[UpdateBefore(typeof(ApplyAttributesDeltaSystem))]
public class HealthAttributeModificationSystem : GameplayEffectAttributeModificationSystem<P_HealthAttributeModifierJob, T_HealthAttributeModifierJob> { }

[UpdateAfter(typeof(ResetAttributesDeltaSystem))]
[UpdateBefore(typeof(ApplyAttributesDeltaSystem))]
public class ManaAttributeModificationSystem : GameplayEffectAttributeModificationSystem<P_ManaAttributeModifierJob, T_ManaAttributeModifierJob> { }


[RequireComponentTag(typeof(AttributeModifyComponent), typeof(HealthAttributeModifier), typeof(PermanentAttributeModification))]
public struct P_HealthAttributeModifierJob : AttributeModifierJob<PermanentAttributeModification> {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    [ReadOnly] private ComponentDataFromEntity<PermanentAttributeModification> _attributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<PermanentAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

    public void Execute(Entity entity, int index, ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Health;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
            attr.BaseValue += attrMod.Change;
            attrs.Health = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
        Ecb.DestroyEntity(index, entity);
    }
}

[RequireComponentTag(typeof(AttributeModifyComponent), typeof(HealthAttributeModifier), typeof(TemporaryAttributeModification))]
public struct T_HealthAttributeModifierJob : AttributeModifierJob<TemporaryAttributeModification> {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    [ReadOnly] private ComponentDataFromEntity<TemporaryAttributeModification> _attributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<TemporaryAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

    public void Execute(Entity entity, int index, ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Health;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
            attr.TempDelta += attrMod.Change;
            attrs.Health = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
    }
}


[RequireComponentTag(typeof(AttributeModifyComponent), typeof(ManaAttributeModifier), typeof(PermanentAttributeModification))]
public struct P_ManaAttributeModifierJob : AttributeModifierJob<PermanentAttributeModification> {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    [ReadOnly] private ComponentDataFromEntity<PermanentAttributeModification> _attributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<PermanentAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Mana;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
            attr.BaseValue += attrMod.Change;
            attrs.Mana = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
        Ecb.DestroyEntity(index, entity);
    }
}

[RequireComponentTag(typeof(AttributeModifyComponent), typeof(ManaAttributeModifier), typeof(TemporaryAttributeModification))]
public struct T_ManaAttributeModifierJob : AttributeModifierJob<TemporaryAttributeModification> {
    public EntityCommandBuffer.Concurrent Ecb { get; set; }
    [NativeDisableContainerSafetyRestriction] [WriteOnly] private ComponentDataFromEntity<AttributesComponent> _attrComponents;
    [ReadOnly] private ComponentDataFromEntity<TemporaryAttributeModification> _attributeModifier;
    public ComponentDataFromEntity<AttributesComponent> AttrComponents { get => _attrComponents; set => _attrComponents = value; }
    public ComponentDataFromEntity<TemporaryAttributeModification> AttributeModifier { get => _attributeModifier; set => _attributeModifier = value; }

    public void Execute(Entity entity, int index, [ReadOnly] ref AttributeModificationComponent attrMod) {
        if (_attrComponents.Exists(attrMod.Target)) {
            var attrs = _attrComponents[attrMod.Target];
            var attr = attrs.Mana;
            attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
            attr.TempDelta += attrMod.Change;
            attrs.Mana = attr;
            _attrComponents[attrMod.Target] = attrs;
        }
    }
}