using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.SkyboxVolume
{
    [RequireComponent(typeof(Camera))]
    public class Skybox3D : MonoBehaviour
    {
        [Tooltip("The main camera in the scene. If null, Camera.main is used.")]
        new public Camera camera;
        [Tooltip("A smaller value here increases the scale of the skybox.")]
        public float movementCoefficient = 0.01f;

        Camera skyCam;
        Transform cameraTransform;

        void Start()
        {
            camera.clearFlags = CameraClearFlags.Depth;
            cameraTransform = camera.transform;
            skyCam = GetComponent<Camera>();
        }

        void OnPreRender()
        {
            if (camera != null)
            {
                skyCam.fieldOfView = camera.fieldOfView;
                transform.rotation = cameraTransform.rotation;
                transform.localPosition = cameraTransform.position * movementCoefficient;
            }
        }
    }
}