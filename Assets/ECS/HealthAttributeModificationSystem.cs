using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct _HealthAttributeModifier : IComponentData, _AttributeModifier {
    public void PermanentAttributeModification(ref _AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Health;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.BaseValue += attrMod.Change;
        attrs.Health = attr;
    }

    public void TemporaryAttributeModification(ref _AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Health;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.TempDelta += attrMod.Change;
        attrs.Health = attr;
    }
}

// public class HealthModificationSystem : _AttributeModificationSystem<HealthAttributeModifier> { }

public interface _AttributeModifier {
    void TemporaryAttributeModification(ref _AttributeModificationComponent attrMod, ref AttributesComponent attrs);
    void PermanentAttributeModification(ref _AttributeModificationComponent attrMod, ref AttributesComponent attrs);
}
