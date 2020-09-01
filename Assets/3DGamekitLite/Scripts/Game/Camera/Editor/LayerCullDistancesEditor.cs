using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gamekit3D.Cameras
{
    [CustomEditor(typeof(LayerCullDistances))]
    public class LayerCullDistancesEditor : Editor
    {
        private LayerCullDistances m_Target;
        private int m_OpenedSettings;

        private int[] remainingQuality;
        private string[] remainingQualityNames;

        void OnEnable()
        {
            m_Target = target as LayerCullDistances;
            m_OpenedSettings = -1;

            if(m_Target.settings == null)
                m_Target.Reset();
            else
            {
                for (int i = 0; i < m_Target.settings.Length; ++i)
                {
                    if (m_Target.settings[i].minimumQualitySetting >= QualitySettings.names.Length)
                    {
                        ArrayUtility.RemoveAt(ref m_Target.settings, i);
                        i--;
                    }
                }
            }

            GetRemainingQualitySetting();
        }

        public override void OnInspectorGUI()
        {
            if (remainingQuality.Length > 0)
            {
                int selected = EditorGUILayout.Popup("Add Quality Settings", -1, remainingQualityNames);
                if (selected != -1)
                {
                    Undo.RecordObject(target, "Added new Quality Setting in LayerCUllDistance");
                    m_Target.AddNewSetting(remainingQuality[selected]);
                    EditorUtility.SetDirty(m_Target);
                    ArrayUtility.RemoveAt(ref remainingQualityNames, selected);
                    ArrayUtility.RemoveAt(ref remainingQuality, selected);
                }
            }

            for (int i = 0; i < m_Target.settings.Length; ++i)
            {
                bool opened = EditorGUILayout.Foldout(m_OpenedSettings == i,
                    "Quality : " + QualitySettings.names[m_Target.settings[i].minimumQualitySetting]);

                if (opened)
                {
                    m_OpenedSettings = i;
                    DrawSetting(i);
                }
                else if(m_OpenedSettings == i)
                {
                    m_OpenedSettings = -1;
                }
            }
        }

        void GetRemainingQualitySetting()
        {
            remainingQuality = new int[QualitySettings.names.Length];
            for (int i = 0; i < remainingQuality.Length; ++i)
                remainingQuality[i] = i;

            for (int i = 0; i < m_Target.settings.Length; ++i)
            {
                if (remainingQuality.Contains(m_Target.settings[i].minimumQualitySetting))
                {
                    ArrayUtility.Remove(ref remainingQuality, m_Target.settings[i].minimumQualitySetting);
                }
            }

            remainingQualityNames = new string[remainingQuality.Length];
            for (int i = 0; i < remainingQuality.Length; ++i)
            {
                remainingQualityNames[i] = QualitySettings.names[remainingQuality[i]];
            }
        }

        void DrawSetting(int index)
        {
            LayerCullDistances.QualitySpecificSettings setting = m_Target.settings[index];

            GUILayout.FlexibleSpace();
            if (m_Target.settings.Length > 1 && GUILayout.Button("Remove", GUILayout.Width(64)))
            {
                Undo.RecordObject(m_Target,
                    "Removed quality setting " + QualitySettings.names[m_Target.settings[index].minimumQualitySetting]);
                ArrayUtility.RemoveAt(ref m_Target.settings, index);
                m_OpenedSettings = -1;
                GetRemainingQualitySetting();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                float nearPlane = EditorGUILayout.FloatField("Near Plane", setting.nearPlane);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed near plane");
                    setting.nearPlane = nearPlane;
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                EditorGUI.BeginChangeCheck();
                float farPlane = EditorGUILayout.FloatField("Far Plane", setting.farPlane);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed far plane");
                    setting.farPlane = farPlane;
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                for (var i = 0; i < setting.distances.Length; i++)
                {
                    var name = LayerMask.LayerToName(i);
                    if (name != "")
                    {
                        EditorGUI.BeginChangeCheck();
                        float newValue = EditorGUILayout.Slider(name + " (" + i.ToString() + ")", setting.distances[i],
                            setting.nearPlane,
                            setting.farPlane);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(target, "Changed culling distance for " + name + " layer");
                            setting.distances[i] = newValue;
                            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                        }
                    }
                }
            }
        }
    }
}