using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FilterLOD : EditorWindow
{
    [MenuItem("Tools/FilterLOD")]
    static void Filter()
    {
        GetWindow<FilterLOD>();
    }

    private List<GameObject> lodGroupWithUnaprented = new List<GameObject>();
    private Vector2 scrollPos;

    private void OnEnable()
    {
        scrollPos = Vector2.zero;
        lodGroupWithUnaprented.Clear();
        var gos = EditorSceneManager.GetActiveScene().GetRootGameObjects();

        for(int i = 0; i < gos.Length; ++i)
        {
            HierarchicalDown(gos[i]);
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < lodGroupWithUnaprented.Count; ++i)
        {
            if (GUILayout.Button(lodGroupWithUnaprented[i].name))
            {
                Selection.activeGameObject = lodGroupWithUnaprented[i];
                EditorGUIUtility.PingObject(lodGroupWithUnaprented[i]);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    void HierarchicalDown(GameObject parent)
    {
        LODGroup grp = parent.GetComponent<LODGroup>();

        if (grp != null)
        {
            int expectMeshRenderer = 0;
            for (int i = 0; i < grp.lodCount; ++i)
            {
                expectMeshRenderer += grp.GetLODs()[i].renderers.Length;
            }

            int actualRenderer = 0;
            foreach (Transform t in grp.transform)
            {
                if (t.GetComponent<Renderer>() != null)
                    actualRenderer += 1;
            }

            if (expectMeshRenderer != actualRenderer)
            {
                lodGroupWithUnaprented.Add(parent);
            }
        }
        else
        {
            for (int i = 0; i < parent.transform.childCount; ++i)
            {
                HierarchicalDown(parent.transform.GetChild(i).gameObject);
            }
        }
    }
}
