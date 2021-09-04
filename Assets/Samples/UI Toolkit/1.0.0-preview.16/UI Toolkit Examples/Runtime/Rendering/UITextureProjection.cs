using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Runtime.Rendering
{
    public class UITextureProjection : MonoBehaviour
    {
        public Camera m_TargetCamera;

        /// <summary>
        /// When using a render texture, this camera will be used to translate screencoodinates to the panel's coordinates
        /// </summary>
        /// <remarks>
        /// If none is set, it will be initialized with Camera.main
        /// </remarks>
        public Camera targetCamera
        {
            get
            {
                if (m_TargetCamera == null)
                    m_TargetCamera = Camera.main;
                return m_TargetCamera;
            }
            set => m_TargetCamera = value;
        }

        public PanelSettings TargetPanel;

        private Func<Vector2, Vector2> m_DefaultRenderTextureScreenTranslation;

        void OnEnable()
        {
            if (TargetPanel != null)
            {
                if (m_DefaultRenderTextureScreenTranslation == null)
                {
                    m_DefaultRenderTextureScreenTranslation = (pos) => ScreenCoordinatesToRenderTexture(pos);
                }

                TargetPanel.SetScreenToPanelSpaceFunction(m_DefaultRenderTextureScreenTranslation);
            }
        }

        void OnDisable()
        {
            //we reset it back to the default behavior
            if (TargetPanel != null)
            {
                TargetPanel.SetScreenToPanelSpaceFunction(null);
            }
        }

        /// <summary>
        /// Transforms a screen position to a position relative to render texture used by a MeshRenderer.
        /// </summary>
        /// <param name="screenPosition">The position in screen coordinates.</param>
        /// <param name="currentCamera">Camera used for 3d object picking</param>
        /// <param name="targetTexture">The texture used by the panel</param>
        /// <returns>Returns the coordinates in texel space, or a position containing NaN values if no hit was recorded or if the hit mesh's material is not using the render texture as their mainTexture</returns>
        private Vector2 ScreenCoordinatesToRenderTexture(Vector2 screenPosition)
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            screenPosition.y = Screen.height - screenPosition.y;
            var cameraRay = targetCamera.ScreenPointToRay(screenPosition);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit))
            {
                return invalidPosition;
            }

            var targetTexture = TargetPanel.targetTexture;
            MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();

            if (rend == null || rend.sharedMaterial.mainTexture != targetTexture)
            {
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;

            //since y screen coordinates are usually inverted, we need to flip them
            pixelUV.y = 1 - pixelUV.y;

            pixelUV.x *= targetTexture.width;
            pixelUV.y *= targetTexture.height;

            return pixelUV;
        }
    }
}
