using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class AutoSetQualitySetting
{
    static AutoSetQualitySetting()
    {
        if (!System.IO.File.Exists(Application.dataPath + "/../Library/QualityDetected"))
        {
            System.IO.File.WriteAllText(Application.dataPath + "/../Library/QualityDetected", "Delete this to trigger a new quality settingsdetection");
            DetectQualitySettings();
        }
    }

    [MenuItem("Kit Tools/Auto Detect Quality Settings")]
    static void DetectQualitySettings()
    {
        EditorApplication.update += Update;

        if (SystemInfo.graphicsMemorySize < 1200)
        {
            QualitySettings.SetQualityLevel(0);
        }
        else if(SystemInfo.graphicsMemorySize < 2048)
        {
            QualitySettings.SetQualityLevel(1);
        }
        else
        {
            QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
        }

        EditorUtility.DisplayDialog("Auto Quality set",
            "Quality Settings were automatically set to " + QualitySettings.names[QualitySettings.GetQualityLevel()] +
            "\nYou can change that setting in the Edit > Project Settings > Quality menu.", "Ok");
    }

    static void Update()
    {
        bool force = QualitySettings.GetQualityLevel() < 1;

        if (force)
        {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var property = type.GetProperty("lowResolutionForAspectRatios");
            var gameviews = Resources.FindObjectsOfTypeAll(type);

            foreach (var gameview in gameviews)
            {
                property.SetValue(gameview, QualitySettings.GetQualityLevel() < 1, null);
            }

            Debug.Log("Low Aspect ratio forced on GameViews");
        }

        EditorApplication.update -= Update;
    }
}
