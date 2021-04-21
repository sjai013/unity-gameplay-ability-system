using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/Damage Numbers")]
public class ShowDamageNumbersEventHandler : AbstractAttributeEventHandler
{

    [SerializeField]
    private AttributeScriptableObject PrimaryAttribute;

    [SerializeField]
    private DamageNumberComponent damageNumberComponent;

    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues)
    {

        var attributeCacheDict = attributeSystem.mAttributeIndexCache;
        if (attributeCacheDict.TryGetValue(PrimaryAttribute, out var primaryAttributeIndex))
        {
            var prevValue = prevAttributeValues[primaryAttributeIndex].CurrentValue;
            var currentValue = currentAttributeValues[primaryAttributeIndex].CurrentValue;

            if (prevValue != currentValue)
            {
                // Instantiate a prefab for displaying the number
                var damageNumber = Instantiate(damageNumberComponent, attributeSystem.gameObject.transform.position, attributeSystem.gameObject.transform.rotation);

                // The prefab has an Initialise method, which allows us to pass in the change magnitude, so we can show the appropriate number
                damageNumber.Initialise(currentValue - prevValue);
            }
        }
    }
}
