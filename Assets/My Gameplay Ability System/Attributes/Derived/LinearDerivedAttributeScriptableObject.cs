using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;


[CreateAssetMenu(menuName = "Gameplay Ability System/Linear Derived Attribute")]
public class LinearDerivedAttributeScriptableObject : AttributeScriptableObject
{
    public AttributeScriptableObject Attribute;
    [SerializeField] private float gradient;
    [SerializeField] private float offset;

    public override AttributeValue CalculateCurrentAttributeValue(AttributeValue attributeValue, List<AttributeValue> otherAttributeValues)
    {
        // Find desired attribute in list
        var baseAttributeValue = otherAttributeValues.Find(x => x.Attribute == this.Attribute);

        // Calculate new value
        attributeValue.BaseValue = (baseAttributeValue.CurrentValue * gradient) + offset;

        attributeValue.CurrentValue = (attributeValue.BaseValue + attributeValue.Modifier.Add) * (attributeValue.Modifier.Multiply + 1);

        if (attributeValue.Modifier.Override != 0)
        {
            attributeValue.CurrentValue = attributeValue.Modifier.Override;
        }
        return attributeValue;
    }
}
