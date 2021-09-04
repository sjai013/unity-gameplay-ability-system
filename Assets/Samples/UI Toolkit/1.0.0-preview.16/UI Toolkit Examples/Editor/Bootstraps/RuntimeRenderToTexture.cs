using System;
using Samples.Runtime.Events;
using Samples.Runtime.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Samples.Utils
{
    public static partial class MenuItems
    {
        [MenuItem("Window/UI Toolkit/Examples/Rendering/RenderTexture 3D (Runtime)")]
        public static void StartRuntimeRenderTexture3D()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            var go = new GameObject("UITextureProjection");
            var projection = go.AddComponent<UITextureProjection>();

            var material = CreateDefaultMaterial(projection.TargetPanel.targetTexture);

            go = new GameObject("Whack-A-Button");
            var component = go.AddComponent<ClickEventSample>();
            component.SetPanelSettings(projection.TargetPanel);

            var cube = CreatePrimitive(PrimitiveType.Cube, material, 0.8f);
            //Let's show the other side of the cube
            cube.transform.localEulerAngles = new Vector3(0, 180, 0);

            CreatePrimitive(PrimitiveType.Cylinder, material, -0.8f);

            EditorApplication.EnterPlaymode();
            EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(component));
        }

        [MenuItem("Window/UI Toolkit/Examples/Rendering/RenderTexture Background (Runtime)")]
        public static void StartRuntimeRenderTextureBackground()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            var go = new GameObject("UI");
            var doc = go.AddComponent<UIDocument>();
            var component = go.AddComponent<RenderTextureBackgroundDemo>();
            doc.panelSettings = component.panelSettings;
            doc.visualTreeAsset = component.visualTreeAsset;

            go = new GameObject("Shapes");

            RenderTexture capsuleRt = component.capsuleRt;

            component.cubeRt = AddCameraSetup(go, PrimitiveType.Cube, null, "Cube", 0);
            component.cylinderRt = AddCameraSetup(go, PrimitiveType.Cylinder, null, "Cylinder", 10);
            component.capsuleRt = AddCameraSetup(go, PrimitiveType.Capsule, capsuleRt, "Capsule", 20);

            // let's send it far away for now
            go.transform.localPosition = new Vector3(100, 100, 100);

            EditorApplication.EnterPlaymode();
            EditorGUIUtility.PingObject(MonoScript.FromMonoBehaviour(component));
        }

        static Material CreateDefaultMaterial(RenderTexture texture)
        {
            Material material = null;
            var pipeline = GraphicsSettings.currentRenderPipeline;
            if (pipeline != null)
            {
                material = Object.Instantiate(pipeline.defaultMaterial);

                if (pipeline.GetType().Name == "HDRenderPipelineAsset")
                {
                    material.SetTexture("_BaseColorMap", texture);
                }
            }
            else
            {
                material = new Material(Shader.Find("Standard"));
            }
            material.mainTexture = texture;

            return material;
        }

        static GameObject CreatePrimitive(PrimitiveType type, Material mt, float offset)
        {
            var primitive = GameObject.CreatePrimitive(type);
            primitive.transform.localPosition = new Vector3(offset, 0.8f, -7);

            var rotator = primitive.AddComponent<Rotator>();
            rotator.rotationSpeed = new Vector3(Random.Range(3.0f, 10.0f), Random.Range(3.0f, 10.0f), Random.Range(3.0f, 10.0f));

            var meshRenderer = primitive.GetComponent<MeshRenderer>();
            if (mt != null)
                meshRenderer.sharedMaterial = mt;

            Component collider = null;

            switch (type)
            {
                case PrimitiveType.Sphere:
                    collider = primitive.GetComponent<SphereCollider>();
                    break;
                case PrimitiveType.Capsule:
                case PrimitiveType.Cylinder:
                    collider = primitive.GetComponent<CapsuleCollider>();
                    break;
                case PrimitiveType.Cube:
                    collider = primitive.GetComponent<BoxCollider>();
                    break;
                default:
                    //probably already using a MeshCollider
                    break;
            }

            if (collider != null)
            {
                Object.DestroyImmediate(collider);
                var meshCollider = primitive.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
            }

            return primitive;
        }

        static RenderTexture AddCameraSetup(GameObject go, PrimitiveType type, RenderTexture rt, string label, float offset)
        {
            GameObject root = new GameObject(label);

            root.transform.parent = go.transform;

            var shape = CreatePrimitive(type, null, 0.0f);

            shape.transform.parent = root.transform;
            shape.transform.localPosition = new Vector3(0, 0, 3);

            var camGo = new GameObject("camera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGo.transform.parent = root.transform;
            cam.transform.localPosition = new Vector3(0, 0, 0);

            if (rt == null)
            {
                rt = new RenderTexture(512, 512, 8, RenderTextureFormat.ARGB32);
            }

            cam.targetTexture = rt;
            root.transform.localPosition = new Vector3(offset, 0, 0);


            return rt;
        }
    }
}
