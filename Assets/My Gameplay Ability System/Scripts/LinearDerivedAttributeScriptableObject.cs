using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Linear Derived Attribute")]
public class LinearDerivedAttributeScriptableObject : AttributeScriptableObject
{
    private AttributeScriptableObject m_BaseAttribute;
    [SerializeField] private AnimationCurve m_MappingCurve;

    public override AttributeValue CalculateAttributeValue(AttributeValue attributeValue, List<AttributeValue> otherAttributeValues)
    {
        // Find desired attribute in list
        AttributeValue baseAttributeValue = otherAttributeValues.Find(x => x.Attribute == m_BaseAttribute);

        // Calculate new base value, using the base attribute value and mapping curve
        attributeValue.BaseValue = m_MappingCurve.Evaluate(baseAttributeValue.CurrentValue);

        // Calculate the current value
        attributeValue.CurrentValue = (attributeValue.BaseValue + attributeValue.Modifier.Add) * (attributeValue.Modifier.Multiply + 1);

        if (attributeValue.Modifier.Override != 0)
        {
            attributeValue.CurrentValue = attributeValue.Modifier.Override;
        }
        return attributeValue;
    }
}
