using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

namespace AbilitySystem
{
    [AddComponentMenu("Gameplay Ability System/Single Attribute Value")]
    public class SingleAttributeValue: MonoBehaviour
    {
        [SerializeField] private AbilitySystemCharacter m_AbilitySystemCharacter;
        [SerializeField] private AttributeScriptableObject m_Attribute;

        public bool GetValue(out AttributeValue attributeValue)
        {
            return m_AbilitySystemCharacter.AttributeSystem.GetAttributeValue(m_Attribute, out attributeValue);
        }

    }
}