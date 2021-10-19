using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using AbilitySystem.Authoring;

[CustomEditor(typeof(GameplayEffect))]
public class GameplayEffectEditor : Editor
{

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