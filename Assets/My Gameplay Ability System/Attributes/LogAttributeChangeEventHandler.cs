using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/Attribute Change Log")]
public class LogAttributeChangeEventHandler : AbstractAttributeEventHandler
{
    [SerializeField]
    private AttributeScriptableObject PrimaryAttribute;
    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues)
    {
        var attributeCacheDict = attributeSystem.mAttributeIndexCache;
        if (attributeCacheDict.TryGetValue(PrimaryAttribute, out var primaryAttributeIndex))
        {
            var prevValue = prevAttributeValues[primaryAttributeIndex].CurrentValue;
            var currentValue = currentAttributeValues[primaryAttributeIndex].CurrentValue;

            if (prevValue != currentValue)
            {
                // If value has changed, log a message to console
                Debug.Log($"{attributeSystem.gameObject.name}: {currentAttributeValues[primaryAttributeIndex].Attribute.Name} modified.  Old Value: {prevValue}.  New Value: {currentValue}.");
            }
        }
    }
}
