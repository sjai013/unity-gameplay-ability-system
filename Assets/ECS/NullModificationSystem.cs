using Unity.Entities;

// public class NullAttributeModificationSystem : _AttributeModificationSystem<NullAttributeModifier> { }

public struct NullAttributeModifier : IComponentData, _AttributeModifier {
    public void PermanentAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {

    }

    public void TemporaryAttributeModification(ref AttributeModificationComponent attrMod, ref AttributesComponent attrs) {

    }
}
