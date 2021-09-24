using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using AttributeSystem.Components;


[CustomEditor(typeof(AttributeSystemComponent))]
public class AttributeSystemComponentEditor : Editor
{
    VisualElement rootElement;

    public void OnEnable()
    {
        rootElement = new VisualElement();

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.sjai013.abilitysystem/Runtime/attribute-system/Editor/AttributeSystemComponent/AttributeSystemComponentEditor.uxml");
        visualTree.CloneTree(rootElement);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.sjai013.abilitysystem/Runtime/attribute-system/Editor/AttributeSystemComponent/AttributeSystemComponentEditor.uss");
        rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();
 
        var iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };
                if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    propertyField.SetEnabled(value: false);
 
                container.Add(propertyField);
            }
            while (iterator.NextVisible(false));
        }
 
        return container;
    }
}