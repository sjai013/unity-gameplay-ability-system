using Unity.Entities;

public class NullAttributeModificationSystem : AttributeModificationSystem<NullAttributeModifier> { }

public struct NullAttributeModifier : IComponentData, AttributeModifier {
    public void PermanentAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {

    }

    public void TemporaryAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {

    }
}
