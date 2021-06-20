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

        AttributeValue baseAttributeValue = default(AttributeValue);
        var attributeValid = false;
        // Find the appropriate attribute in the list
        for (var i = 0; i < otherAttributeValues.Count; i++)
        {
            if (otherAttributeValues[i].Attribute == this.Attribute)
            {
                baseAttributeValue = otherAttributeValues[i];
                attributeValid = true;
                break;
            }
        }

        if (!attributeValid) return new AttributeValue();

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
