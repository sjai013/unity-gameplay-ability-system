using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

public struct ManaAttributeModifier : IComponentData, _AttributeModifier {
    public void PermanentAttributeModification(ref _AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Mana;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.BaseValue += attrMod.Change;
        attrs.Mana = attr;
    }

    public void TemporaryAttributeModification(ref _AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Mana;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.TempDelta += attrMod.Change;
        attrs.Mana = attr;
    }
}

// public class ManaModificationSystem : _AttributeModificationSystem<ManaAttributeModifier> { }