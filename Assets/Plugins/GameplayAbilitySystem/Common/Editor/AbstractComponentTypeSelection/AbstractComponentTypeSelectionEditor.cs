/*
 * Created on Mon Nov 04 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameplayAbilitySystem.Common.Editor {

    public class AbilitySystemDisplayNameAttribute : System.Attribute {
        public string Name { get; set; }
        public AbilitySystemDisplayNameAttribute(string name) {
            this.Name = name;
        }
    }

    public abstract class AbstractComponentTypeSelectionEditor<T> : UnityEditor.Editor {
        private VisualElement m_RootElement;
        private VisualTreeAsset m_ModulesVisualTree;
        private List<T> components;

        const string baseAssetPath = "Assets/Plugins/GameplayAbilitySystem/Common/Editor/AbstractComponentTypeSelection/";

        // Start is called before the first frame update
        public void OnEnable() {

            m_RootElement = new VisualElement();
            m_ModulesVisualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    baseAssetPath + "/AbstractComponentTypeSelectionEditor.uxml"
                );
            var stylesheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    baseAssetPath + "/AbstractComponentTypeSelectionEditor.uss"
                );
            m_RootElement.styleSheets.Add(stylesheet);

            // Cleanup any strings which correspond to types that no longer exist.
            var allTypes = new ComponentCollector().GetAllTypes(System.AppDomain.CurrentDomain);
            var serializedTypeStrings = new List<string>();
            var componentsSerialized = serializedObject.FindProperty("Components");
            var count = componentsSerialized.arraySize;
            for (var i = count - 1; i >= 0; i--) {
                var type = componentsSerialized.GetArrayElementAtIndex(i).stringValue;
                if (!allTypes.Any(x => x.AssemblyQualifiedName == type)) {
                    componentsSerialized.DeleteArrayElementAtIndex(i);
                }
            }
            serializedObject.ApplyModifiedProperties();
            // Go through each item in list and check if it in allTypes.  If it isn't, delete.
        }

        public override VisualElement CreateInspectorGUI() {
            var container = m_RootElement;
            container.Clear();
            m_ModulesVisualTree.CloneTree(container);
            var allTypes = new ComponentCollector().GetAllTypes(System.AppDomain.CurrentDomain);

            var componentsSerialized = serializedObject.FindProperty("Components");
            var serializedTypeStrings = new List<string>();
            var count = componentsSerialized.arraySize;
            for (var i = 0; i < count; i++) {
                serializedTypeStrings.Add(componentsSerialized.GetArrayElementAtIndex(i).stringValue);
            }

            foreach (var type in allTypes) {
                // Look for a "DisplayName" attribute
                var displayNameAttribute = (AbilitySystemDisplayNameAttribute)System.Attribute.GetCustomAttribute(type, typeof(AbilitySystemDisplayNameAttribute));
                string displayName = "";
                if (displayNameAttribute != null) {
                    displayName = displayNameAttribute.Name;
                }

                var button = new Button(() => {
                    var existingIndex = serializedTypeStrings.FindIndex(x => x == type.AssemblyQualifiedName);
                    // if this already exists in the list, delete it
                    if (existingIndex >= 0) {
                        componentsSerialized.DeleteArrayElementAtIndex(existingIndex);
                        serializedObject.ApplyModifiedProperties();
                    } else {
                        // Add it to list
                        componentsSerialized.InsertArrayElementAtIndex(0);
                        componentsSerialized.GetArrayElementAtIndex(0).stringValue = type.AssemblyQualifiedName;
                        serializedObject.ApplyModifiedProperties();
                    }
                    CreateInspectorGUI();
                })
                { text = displayName == "" ? type.FullName : displayName };

                // If this type is in the list of selected components, mark it enabled
                if (serializedTypeStrings.Any(x => x == type.AssemblyQualifiedName)) {
                    button.AddToClassList("enabled-button");
                }

                container.Add(button);
            }



            return container;
        }
        public class ComponentCollector {
            public IEnumerable<System.Type> GetAllTypes(System.AppDomain domain) {
                var componentInterface = typeof(T);
                var types = domain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => componentInterface.IsAssignableFrom(p) && !p.IsInterface);

                return types;
            }

        }
    }
}
