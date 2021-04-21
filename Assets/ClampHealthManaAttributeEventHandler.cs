using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/Clamp Attribute")]
public class ClampHealthManaAttributeEventHandler : AbstractAttributeEventHandler
{

    [SerializeField]
    private AttributeScriptableObject HealthAttribute;

    [SerializeField]
    private AttributeScriptableObject MaxHealthAttribute;

    [SerializeField]
    private AttributeScriptableObject ManaAttribute;

    [SerializeField]
    private AttributeScriptableObject MaxManaAttribute;
    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues)
    {

        var attributeCacheDict = attributeSystem.mAttributeIndexCache;
        ClampAttributeToMax(HealthAttribute, MaxHealthAttribute, currentAttributeValues, attributeCacheDict);
        ClampAttributeToMax(ManaAttribute, MaxManaAttribute, currentAttributeValues, attributeCacheDict);

    }

    private void ClampAttributeToMax(AttributeScriptableObject PrimaryAttribute, AttributeScriptableObject MaxAttribute, List<AttributeValue> attributeValues, Dictionary<AttributeScriptableObject, int> attributeCacheDict)
    {
        if (attributeCacheDict.TryGetValue(PrimaryAttribute, out var primaryAttributeIndex)
            && attributeCacheDict.TryGetValue(MaxAttribute, out var maxAttributeIndex))
        {
            var primaryAttribute = attributeValues[primaryAttributeIndex];
            var maxAttribute = attributeValues[maxAttributeIndex];

            if (primaryAttribute.CurrentValue > maxAttribute.CurrentValue) primaryAttribute.CurrentValue = maxAttribute.CurrentValue;
            if (primaryAttribute.BaseValue > maxAttribute.BaseValue) primaryAttribute.BaseValue = maxAttribute.BaseValue;
            attributeValues[primaryAttributeIndex] = primaryAttribute;
        }
    }



}