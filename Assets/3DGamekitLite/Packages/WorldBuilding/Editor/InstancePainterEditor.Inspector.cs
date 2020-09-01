using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;


namespace Gamekit3D.WorldBuilding
{

    public partial class InstancePainterEditor : Editor
    {
        [MenuItem("GameObject/Create Other/Instance Painter")]
        static void CreateInstancePainter()
        {
            var g = new GameObject("Instance Painter", typeof(InstancePainter));
            Selection.activeGameObject = g;
        }

        void RefreshPaletteImages(InstancePainter ip)
        {
            if (palleteImages == null || palleteImages.Length != ip.prefabPallete.Length)
            {
                palleteImages = new Texture2D[ip.prefabPallete.Length];
                for (var i = 0; i < ip.prefabPallete.Length; i++)
                    palleteImages[i] = AssetPreview.GetAssetPreview(ip.prefabPallete[i]);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (ip.rootTransform == null)
            {
                EditorGUILayout.HelpBox("You must assign the root transform for new painted instances.", MessageType.Error);
                ip.rootTransform = (Transform)EditorGUILayout.ObjectField("Root Transform", ip.rootTransform, typeof(Transform), true);
                return;
            }
            EditorGUILayout.HelpBox("Stamp: Left Click\nErase: Ctrl + Left Click\nRotate: Shift + Scroll\nBrush Size: Alt + Scroll or [ and ]\nDensity: - =\nScale: . /\nSpace: Randomize", MessageType.Info);
            base.OnInspectorGUI();
            if (ip.prefabPallete == null || ip.prefabPallete.Length == 0)
            {
                EditorGUILayout.HelpBox("You must assign prefabs to the Prefab Pallete array.", MessageType.Error);
                return;
            }
            GUILayout.Space(16);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Align to Normal");
                ip.alignToNormal = GUILayout.Toggle(ip.alignToNormal, GUIContent.none);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Follow Surface");
                ip.followOnSurface = GUILayout.Toggle(ip.followOnSurface, GUIContent.none);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Randomize each Stamp");
                ip.randomizeAfterStamp = GUILayout.Toggle(ip.randomizeAfterStamp, GUIContent.none);
            }

            GUILayout.Space(16);
            if (ip.prefabPallete != null && ip.prefabPallete.Length > 0)
            {
                RefreshPaletteImages(ip);
                var tileSize = 96;
                var xCount = Mathf.FloorToInt(Screen.width / tileSize + 1);
                var gridHeight = GUILayout.Height(palleteImages.Length / (xCount) * tileSize);
                var newIndex = GUILayout.SelectionGrid(ip.selectedPrefabIndex, palleteImages, xCount, EditorStyles.miniButton, gridHeight);
                if (newIndex != ip.selectedPrefabIndex)
                {
                    ip.selectedPrefabIndex = newIndex;
                    variations = ip.SelectedPrefab.GetComponent<Variations>();
                    if (variationsEditor != null)
                        DestroyImmediate(variationsEditor);
                    if (variations != null)
                        variationsEditor = Editor.CreateEditor(variations);
                    CreateNewStamp();
                }
                GUILayout.Space(16);
                if (variationsEditor == null)
                {
                    if (GUILayout.Button("Add Variations"))
                    {
                        variations = ip.SelectedPrefab.AddComponent<Variations>();
                        variationsEditor = Editor.CreateEditor(variations);
                    }
                }
                else
                {
                    variationsEditor.OnInspectorGUI();
                }

            }
        }

    }
}