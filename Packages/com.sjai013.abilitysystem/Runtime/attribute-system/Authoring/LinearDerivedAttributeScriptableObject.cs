using System.Collections.Generic;
using AttributeSystem.Components;
using UnityEngine;

namespace AttributeSystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Linear Derived Attribute")]
    public class LinearDerivedAttributeScriptableObject : AttributeScriptableObject
    {
        [SerializeField] private AttributeScriptableObject m_BaseAttribute;
        [SerializeField] private AnimationCurve m_MappingCurve;

        private int m_AttributeIndexCache = -1;

        public override AttributeValue CalculateAttributeValue(AttributeValue attributeValue, List<AttributeValue> otherAttributeValues)
        {
            // Find desired attribute in list
            AttributeValue? baseAttributeValue = null;

            if (!(m_AttributeIndexCache >= 0
                && m_AttributeIndexCache < otherAttributeValues.Count - 1
                && otherAttributeValues[m_AttributeIndexCache].Attribute == m_BaseAttribute))
            {
                m_AttributeIndexCache = FindAttributeIndexInList(attributeValue, otherAttributeValues);
            }

            if (m_AttributeIndexCache < 0)
            {
                Debug.LogWarning("Attempt to reference non-existent attribute.  Ignoring.");
                m_AttributeIndexCache = -1;
                return attributeValue;
            }
            
            baseAttributeValue = otherAttributeValues[m_AttributeIndexCache];


            // Calculate new base value, using the base attribute value and mapping curve
            attributeValue.BaseValue = m_MappingCurve.Evaluate(baseAttributeValue.Value.CurrentValue);

            // Calculate the current value
            attributeValue.CurrentValue = (attributeValue.BaseValue + attributeValue.Modifier.Add) * (attributeValue.Modifier.Multiply + 1);

            if (attributeValue.Modifier.Override != 0)
            {
                attributeValue.CurrentValue = attributeValue.Modifier.Override;
            }
            return attributeValue;
        }

        public int FindAttributeIndexInList(AttributeValue attributeValue, List<AttributeValue> otherAttributeValues)
        {
            for (var i = 0; i < otherAttributeValues.Count; i++)
            {
                if (otherAttributeValues[i].Attribute == m_BaseAttribute)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
