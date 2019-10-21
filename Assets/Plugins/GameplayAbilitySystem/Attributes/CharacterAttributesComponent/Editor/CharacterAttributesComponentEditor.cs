using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitySystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameplayAbilitySystem.Attributes.Components {
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
                    "Assets/Plugins/GameplayAbilitySystem/Attributes/CharacterAttributesComponent/Editor/CharacterAttributesComponentEditor.uxml"
                );
            var stylesheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Plugins/GameplayAbilitySystem/Attributes/CharacterAttributesComponent/Editor/CharacterAttributesComponentEditor.uss"
                );
            m_RootElement.styleSheets.Add(stylesheet);
            
            // Cleanup any strings which correspond to types that no longer exist.
            var allTypes = new AttributeCollector().GetAllAttributeClasses(System.AppDomain.CurrentDomain);
            var serializedTypeStrings = new List<string>();
            var attributesSerialized = serializedObject.FindProperty("Attributes");
            var count = attributesSerialized.arraySize;
            for (var i = count - 1; i >= 0; i--) {
                var type = attributesSerialized.GetArrayElementAtIndex(i).stringValue;
                if (!allTypes.Any(x => x.AssemblyQualifiedName == type)) {
                    attributesSerialized.DeleteArrayElementAtIndex(i);
                }
            }
            serializedObject.ApplyModifiedProperties();
            // Go through each item in list and check if it in allTypes.  If it isn't, delete.
        }

        public override VisualElement CreateInspectorGUI() {
            var container = m_RootElement;
            container.Clear();
            m_ModulesVisualTree.CloneTree(container);
            var allTypes = new AttributeCollector().GetAllAttributeClasses(System.AppDomain.CurrentDomain);

            var attributesSerialized = serializedObject.FindProperty("Attributes");
            var serializedTypeStrings = new List<string>();
            var count = attributesSerialized.arraySize;
            for (var i = 0; i < count; i++) {
                serializedTypeStrings.Add(attributesSerialized.GetArrayElementAtIndex(i).stringValue);
            }

            foreach (var type in allTypes) {
                var button = new Button(() => {
                    var existingIndex = serializedTypeStrings.FindIndex(x => x == type.AssemblyQualifiedName);
                    // if this already exists in the list, delete it
                    if (existingIndex >= 0) {
                        attributesSerialized.DeleteArrayElementAtIndex(existingIndex);
                        serializedObject.ApplyModifiedProperties();
                    } else {
                        // Add it to list
                        attributesSerialized.InsertArrayElementAtIndex(0);
                        attributesSerialized.GetArrayElementAtIndex(0).stringValue = type.AssemblyQualifiedName;
                        serializedObject.ApplyModifiedProperties();
                    }
                    CreateInspectorGUI();
                })
                { text = type.Name };

                // If this type is in the list of selected attributes, mark it enabled
                if (serializedTypeStrings.Any(x => x == type.AssemblyQualifiedName)) {
                    button.AddToClassList("enabled-button");
                }

                container.Add(button);
            }



            return container;
        }
        public class AttributeCollector {

            public IEnumerable<System.Type> GetAllAttributeClasses(System.AppDomain domain) {
                var attributeInterface = typeof(IAttributeComponent);
                var types = domain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => attributeInterface.IsAssignableFrom(p) && !p.IsInterface);

                return types;
            }

        }
    }
}
