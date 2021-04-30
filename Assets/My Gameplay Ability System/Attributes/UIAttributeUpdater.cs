using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIAttributeUpdater : MonoBehaviour
{
    [SerializeField]
    private AttributeSystemComponent attributeSystemComponent;

    [SerializeField]
    private BaseAttributeUIComponent attributeUI;

    [SerializeField]
    private AttributeScriptableObject currentAttribute;

    [SerializeField]
    private AttributeScriptableObject maxAttribute;

    void LateUpdate()
    {
        if (!attributeSystemComponent) return;
        if (!attributeUI) return;
        if (!currentAttribute) return;
        if (!maxAttribute) return;

        if (attributeSystemComponent.GetAttributeValue(currentAttribute, out var currentAttributeValue)
            && attributeSystemComponent.GetAttributeValue(maxAttribute, out var maxAttributeValue))
        {
            attributeUI.SetAttributeValue(currentAttributeValue.CurrentValue, maxAttributeValue.CurrentValue);
        }
    }
}
