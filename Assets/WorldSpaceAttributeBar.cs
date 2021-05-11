using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldSpaceAttributeBar : MonoBehaviour
{
    [SerializeField]
    private Transform HealthBarPosition;

    [SerializeField]
    private AttributeScriptableObject attribute;


    [SerializeField]
    private AttributeScriptableObject attributeMax;

    [SerializeField]
    private AttributeSystemComponent attributeSystem;

    [SerializeField]
    private Color attributeColour;

    private VisualElement bar;
    private VisualElement container;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        container = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("container");
        bar = container.Q<VisualElement>("attribute-value");

        SetBarColour();
        FaceCamera();
        UpdateAttribute();
    }

    void SetBarColour()
    {
        bar.style.backgroundColor = attributeColour;
    }
    void LateUpdate()
    {
        FaceCamera();
        UpdateAttribute();
    }

    void UpdateAttribute()
    {
        if (attributeSystem == null) return;
        if (attribute == null) return;
        if (attributeMax == null) return;

        attributeSystem.GetAttributeValue(attribute, out var attributeValue);
        attributeSystem.GetAttributeValue(attributeMax, out var attributeMaxValue);

        var percent = attributeMaxValue.CurrentValue != 0 ? attributeValue.CurrentValue / attributeMaxValue.CurrentValue : 0;
        bar.style.width = Length.Percent(percent * 100);
    }

    void FaceCamera()
    {
        if (HealthBarPosition == null) return;
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(container.panel, HealthBarPosition.position, mainCamera);

        container.transform.position = new Vector3()
        {
            x = newPosition.x - container.layout.width / 2,
            y = newPosition.y - container.layout.height / 2,
            z = container.transform.position.z
        };
    }

}
