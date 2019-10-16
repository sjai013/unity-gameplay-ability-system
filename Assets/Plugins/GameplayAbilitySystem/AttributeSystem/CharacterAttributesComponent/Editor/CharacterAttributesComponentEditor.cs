using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CharacterAttributesComponent))]
public class CharacterAttributesComponentEditor : Editor {
    private VisualElement m_RootElement;
    private VisualTreeAsset m_ModulesVisualTree;

    private List<IAttributeComponent> attributeComponents;

    // Start is called before the first frame update
    public void OnEnable() {
        m_RootElement = new VisualElement();
        m_ModulesVisualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/Plugins/GameplayAbilitySystem/AttributeSystem/CharacterAttributesComponent/Editor/CharacterAttributesComponentEditor.uxml"
            );
        var stylesheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Plugins/GameplayAbilitySystem/AttributeSystem/CharacterAttributesComponent/Editor/CharacterAttributesComponentEditor.uss"
            );
        m_RootElement.styleSheets.Add(stylesheet);




    }

    public override VisualElement CreateInspectorGUI() {
        var container = m_RootElement;
        container.Clear();
        m_ModulesVisualTree.CloneTree(container);

        // Get a reference to the field from UXML and assign it its value.
        var uxmlField = container.Q<Toggle>("the-uxml-field");
        uxmlField.value = true;

        // Create a new field, disable it, and give it a style class.
        var csharpField = new Toggle("C# Field");
        csharpField.value = false;
        csharpField.SetEnabled(false);
        csharpField.AddToClassList("some-styled-field");
        csharpField.value = uxmlField.value;
        container.Add(csharpField);

        // Mirror value of uxml field into the C# field.
        uxmlField.RegisterCallback<ChangeEvent<bool>>((evt) => {
            csharpField.value = evt.newValue;
        });

        return container;
    }
}
