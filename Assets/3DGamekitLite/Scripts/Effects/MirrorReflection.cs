using UnityEngine;
using System.Collections;

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class MirrorReflection : MonoBehaviour
    {
        public Texture lowQualityTexture;
        private static bool insideRendering = false;
        public Camera mainCamera;
        public Camera reflectionCamera;
        public int textureSize = 256;
        public float clipPlaneOffset = 0.07f;
        RenderTexture reflectionTexture = null;
        int reflectionTexID;


        void OnEnable()
        {
            if(mainCamera != null)
                mainCamera.depthTextureMode = DepthTextureMode.Depth;

            reflectionTexture = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.ARGB32);
            reflectionTexID = Shader.PropertyToID("_ReflectionTex");
            //IMPORTANT: reflection camera is disabled, otherwise it will render twice, as it is called manually.
            reflectionCamera.enabled = false;
        }

        void OnDisable()
        {
            if (reflectionCamera != null) reflectionCamera.targetTexture = null;
            if (reflectionTexture != null) DestroyImmediate(reflectionTexture);

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                var materials = renderer.sharedMaterials;
                for (var i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    mat.SetTexture(reflectionTexID, lowQualityTexture);
                }
            }
        }

        public void OnWillRenderObject()
        {
            var renderer = GetComponent<Renderer>();
            var camera = Camera.current;
            if (!camera) return;
            if (insideRendering)
                return;
            insideRendering = true;
            //if (camera != lastCamera)
            {
                reflectionCamera.fieldOfView = camera.fieldOfView;
                reflectionCamera.layerCullSpherical = true;
                reflectionCamera.layerCullDistances = camera.layerCullDistances;
                reflectionCamera.farClipPlane = camera.farClipPlane;
            }

            // find out the reflection plane: position and normal in world space
            var pos = transform.position;
            var normal = transform.up;

            // Render reflection
            // Reflect camera around reflection plane
            var d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
            var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            var reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            var oldpos = camera.transform.position;
            var newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = camera.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            var clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
            //Matrix4x4 projection = cam.projectionMatrix;
            var projection = camera.CalculateObliqueMatrix(clipPlane);
            reflectionCamera.projectionMatrix = projection;

            reflectionCamera.targetTexture = reflectionTexture;
            GL.invertCulling = true;
            reflectionCamera.transform.position = newpos;
            var euler = camera.transform.eulerAngles;
            reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
            reflectionCamera.Render();

            GL.invertCulling = false;
            var materials = renderer.sharedMaterials;
            for (var i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                mat.SetTexture(reflectionTexID, reflectionTexture);
            }
            insideRendering = false;
        }

        // Given position/normal of the plane, calculates plane in camera space.
        Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            var offsetPos = pos + normal * clipPlaneOffset;
            var m = cam.worldToCameraMatrix;
            var cpos = m.MultiplyPoint(offsetPos);
            var cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        // Calculates reflection matrix around the given plane
        static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;
        }
    }
}