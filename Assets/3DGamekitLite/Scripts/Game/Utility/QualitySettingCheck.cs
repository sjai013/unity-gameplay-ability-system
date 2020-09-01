using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamekit3D
{
    //This will disabled the targets Monobehaviour if a given quality setting isn't met
    [ExecuteInEditMode]
    public class QualitySettingCheck : MonoBehaviour
    {
        public MonoBehaviour[] targets;

        [HideInInspector]
        public int minimumQualitySettings;

        protected int m_PreviousQualitySetting;

        void OnEnable()
        {
            m_PreviousQualitySetting = QualitySettings.GetQualityLevel();
            Toggle(m_PreviousQualitySetting >= minimumQualitySettings);
        }

        void Update()
        {
            //This is *slightly* expensive as we are going to check on every QulaitySettingChecker every frame (though our project only have a couple)
            //But this detect quality changes esily. A real world scenario would have a system that register every behaviour that need to be notified
            //when quality change, and that system will be use to modify quality setting and notify objects.
            if (m_PreviousQualitySetting != QualitySettings.GetQualityLevel())
            {
                m_PreviousQualitySetting = QualitySettings.GetQualityLevel();
                Toggle(m_PreviousQualitySetting >= minimumQualitySettings);
            }
        }

        void Toggle(bool qualitySettingMet)
        {
            for (int i = 0; i < targets.Length; ++i)
            {
                targets[i].enabled = qualitySettingMet;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(QualitySettingCheck))]
    public class QualitySettingCheckEditor : Editor
    {
        private QualitySettingCheck m_Target;

        void OnEnable()
        {
            m_Target = target as QualitySettingCheck;

            if (m_Target.minimumQualitySettings >= QualitySettings.names.Length)
                m_Target.minimumQualitySettings = 0;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            int selected = EditorGUILayout.Popup("Minimum quality setting : ", m_Target.minimumQualitySettings, QualitySettings.names);
            if (selected != m_Target.minimumQualitySettings)
            {
                Undo.RecordObject(m_Target, "Changed minimum quality settings");
                m_Target.minimumQualitySettings = selected;
                EditorUtility.SetDirty(m_Target);
            }
        }
    }
#endif
}