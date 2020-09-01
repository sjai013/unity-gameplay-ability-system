using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamekit3D
{
    public class NewSceneCreator : EditorWindow
    {
        protected string m_NewSceneName;

        protected readonly GUIContent m_NameContent = new GUIContent("New Scene Name");

        [MenuItem("Kit Tools/Create New Scene...", priority = 100)]
        static void Init()
        {
            NewSceneCreator window = GetWindow<NewSceneCreator>();
            window.Show();
        }

        void OnGUI()
        {
            m_NewSceneName = EditorGUILayout.TextField(m_NameContent, m_NewSceneName);

            if (GUILayout.Button("Create"))
                CheckAndCreateScene();
        }

        protected void CheckAndCreateScene()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot create scenes while in play mode.  Exit play mode first.");
                return;
            }

            Scene currentActiveScene = SceneManager.GetActiveScene();

            if (currentActiveScene.isDirty)
            {
                string title = currentActiveScene.name + " Has Been Modified";
                string message = "Do you want to save the changes you made to " + currentActiveScene.path + "?\nChanges will be lost if you don't save them.";
                int option = EditorUtility.DisplayDialogComplex(title, message, "Save", "Don't Save", "Cancel");

                if (option == 0)
                {
                    EditorSceneManager.SaveScene(currentActiveScene);
                }
                else if (option == 2)
                {
                    return;
                }
            }

            CreateScene();

        }

        protected void CreateScene()
        {
            string[] result = AssetDatabase.FindAssets("_TemplateScene");

            if (result.Length > 0)
            {
                string newScenePath = "Assets/" + m_NewSceneName + ".unity";
                AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(result[0]), newScenePath);
                AssetDatabase.Refresh();
                Scene newScene = EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
                AddSceneToBuildSettings(newScene);
                Close();
            }
            else
            {
                //Debug.LogError("The template scene <b>_TemplateScene</b> couldn't be found ");
                EditorUtility.DisplayDialog("Error",
                    "The scene _TemplateScene was not found in Gamekit3D/Scenes folder. This scene is required by the New Scene Creator.",
                    "OK");
            }
        }

        protected void AddSceneToBuildSettings(Scene scene)
        {
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
            for (int i = 0; i < buildScenes.Length; i++)
            {
                newBuildScenes[i] = buildScenes[i];
            }
            newBuildScenes[buildScenes.Length] = new EditorBuildSettingsScene(scene.path, true);
            EditorBuildSettings.scenes = newBuildScenes;
        }

        protected GameObject InstantiatePrefab(string folderPath, string prefabName)
        {
            GameObject instance = null;
            string[] prefabFolderPath = { folderPath };
            string[] guids = AssetDatabase.FindAssets(prefabName, prefabFolderPath);

            if (guids.Length == 0)
                Debug.LogError("The " + prefabName + " prefab could not be found in " + folderPath + " and could therefore not be instantiated.  Please create one manually.");
            else if (guids.Length > 1)
                Debug.LogError("Multiple " + prefabName + " prefabs were found in " + folderPath + " and one could therefore not be instantiated.  Please create one manually.");
            else
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                instance = Instantiate(prefab);
            }

            return instance;
        }
    }
}