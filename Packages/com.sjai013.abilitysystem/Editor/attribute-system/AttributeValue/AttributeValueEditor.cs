using UnityEditor;
using UnityEngine.UIElements;
using AttributeSystem.Components;
using UnityEngine;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(AttributeValue))]
public class AttributeValueEditor : PropertyDrawer
{

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Each editor window contains a root VisualElement object
        VisualElement rootElement = new VisualElement();
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.sjai013.abilitysystem/Runtime/attribute-system/Editor/AttributeValue/AttributeValueEditor.uxml");
        visualTree.CloneTree(rootElement);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.sjai013.abilitysystem/Runtime/attribute-system/Editor/AttributeValue/AttributeValueEditor.uss");
        rootElement.styleSheets.Add(styleSheet);

        var changeValueElement = rootElement.Q<FloatField>("txtChange");
        var btnIncrementElement = rootElement.Q<Button>("btnIncrement");
        var btnDecrementElement = rootElement.Q<Button>("btnDecrement");
        var btnResetElement = rootElement.Q<Button>("btnReset");

        btnIncrementElement.clicked += () =>
        {
            var number = changeValueElement.value;
            ChangeBaseValue(property, number);
        };

        btnDecrementElement.clicked += () =>
        {
            var number = changeValueElement.value * -1;
            ChangeBaseValue(property, number);
        };

        btnResetElement.clicked += () =>
        {
            SetBaseValue(property, 0);
        };


        return rootElement;

        static void ChangeBaseValue(SerializedProperty property, float number)
        {
            property.FindPropertyRelative("BaseValue").floatValue += number;
            property.serializedObject.ApplyModifiedProperties();
        }

        static void SetBaseValue(SerializedProperty property, float number)
        {
            property.FindPropertyRelative("BaseValue").floatValue = number;
            property.serializedObject.ApplyModifiedProperties();
        }
    }

}