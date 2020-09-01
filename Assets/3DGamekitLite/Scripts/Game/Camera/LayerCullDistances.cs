using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamekit3D.Cameras
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class LayerCullDistances : MonoBehaviour
    {
        [System.Serializable]
        public class QualitySpecificSettings
        {
            public int minimumQualitySetting = 0;
            public float nearPlane = 0.3f;
            public float farPlane = 1000f;
            public float[] distances = new float[32];
        }

        public new Camera camera;

        public QualitySpecificSettings[] settings = new QualitySpecificSettings[0];

        private int pickedSetting;
        float[] computedDistances;

#if UNITY_EDITOR
        public void Reset()
        {
            settings = new QualitySpecificSettings[0];

            AddNewSetting(0);
        }

        public void AddNewSetting(int settingLevel)
        {
            var setting = new QualitySpecificSettings();

            setting.nearPlane = 0.3f;
            setting.farPlane = 5000.0f;
            setting.minimumQualitySetting = settingLevel;

            for (int i = 0; i < 32; ++i)
            {
                setting.distances[i] = 1500;
            }

            UnityEditor.ArrayUtility.Add(ref settings, setting);
        }
#endif

        private void OnEnable()
        {
            camera = GetComponent<Camera>();
        }

        void Start()
        {
            ComputeLayerCullDistances();
        }

        public void ComputeLayerCullDistances()
        {
            FindSettings();

            if (pickedSetting < 0 || pickedSetting >= settings.Length)
                return;

            camera.farClipPlane = settings[pickedSetting].farPlane;
            camera.nearClipPlane = settings[pickedSetting].nearPlane;
            camera.layerCullDistances = settings[pickedSetting].distances;
            camera.layerCullSpherical = true;

            computedDistances = new float[settings[pickedSetting].distances.Length];
            for (int i = 0; i < settings[pickedSetting].distances.Length; ++i)
                computedDistances[i] = settings[pickedSetting].distances[i] / (settings[pickedSetting].farPlane - settings[pickedSetting].nearPlane);

            Shader.SetGlobalFloatArray("_LayerCullDistances", computedDistances);
        }

        void FindSettings()
        {
            int foundIdx = -1;
            int highestSetting = -1;
            int currentQualitySetting = QualitySettings.GetQualityLevel();

            for (int i = 0; i < settings.Length; ++i)
            {
                if (settings[i].minimumQualitySetting <= currentQualitySetting &&
                    settings[i].minimumQualitySetting > highestSetting)
                {
                    highestSetting = settings[i].minimumQualitySetting;
                    foundIdx = i;
                }
            }

            if (foundIdx == -1)
            {//use the first one
                pickedSetting = 0;
            }
            else
            {
                pickedSetting = foundIdx;
            }
        }

        void Update()
        {
            if (Application.isEditor)
                ComputeLayerCullDistances();
        }
    }
}