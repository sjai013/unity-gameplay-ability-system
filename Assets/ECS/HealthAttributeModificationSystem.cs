using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

public struct HealthAttributeModifier : IComponentData, AttributeModifier {
    public void PermanentAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Health;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.BaseValue += attrMod.Change;
        attrs.Health = attr;
    }

    public void TemporaryAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {
        var attr = attrs.Health;
        attrMod.Change = attrMod.Add + (attr.BaseValue * attrMod.Multiply) + (attrMod.Divide != 0 ? attr.BaseValue / attrMod.Divide : 0);
        attr.TempDelta += attrMod.Change;
        attrs.Health = attr;
    }
}

public class HealthModificationSystem : AttributeModificationSystem<HealthAttributeModifier> { }

public interface AttributeModifier {
    void TemporaryAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs);
    void PermanentAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs);
}
