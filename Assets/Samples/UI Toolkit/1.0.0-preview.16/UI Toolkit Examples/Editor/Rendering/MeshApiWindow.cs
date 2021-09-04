using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Editor.Rendering
{
    public class MeshApiWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/Examples/Rendering/Mesh API (Editor)")]
        public static void OpenWindow()
        {
            var window = GetWindow<MeshApiWindow>("Mesh API Sample");
            window.minSize = new Vector2(350, 150);
            EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(window));
        }

        List<IDisposable> m_ToDispose = new List<IDisposable>();

        void OnEnable()
        {
            IStyle rootStyle = rootVisualElement.style;
            rootStyle.flexDirection = FlexDirection.Row;
            rootStyle.flexWrap = Wrap.Wrap;

            rootVisualElement.Add(new SolidQuad { name = "RedGradient", style = { width = 100, height = 100, color = Color.red } });
            rootVisualElement.Add(new SolidQuad { name = "GreenGradient", style = { width = 100, height = 100, color = Color.green } });
            rootVisualElement.Add(new SolidQuad { name = "BlueGradient", style = { width = 100, height = 100, color = Color.blue } });
            rootVisualElement.Add(new SolidHexagon { name = "Hexagon", style = { width = 100, height = 100 } });
            rootVisualElement.Add(new TexturedCheckerboard { name = "Checkerboard", style = { width = 100, height = 100 } });
        }

        void OnDisable()
        {
            rootVisualElement.Clear();
        }

        class SolidQuad : VisualElement
        {
            public SolidQuad() { generateVisualContent += OnGenerateVisualContent; }

            static readonly Vertex[] k_Vertices = new Vertex[4];
            static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };

            void OnGenerateVisualContent(MeshGenerationContext mgc)
            {
                Rect r = contentRect;
                if (r.width < 0.01f || r.height < 0.01f)
                    return; // Skip rendering when too small.

                Color color = resolvedStyle.color;
                k_Vertices[0].tint = Color.black;
                k_Vertices[1].tint = Color.black;
                k_Vertices[2].tint = color;
                k_Vertices[3].tint = color;

                float left = 0;
                float right = r.width;
                float top = 0;
                float bottom = r.height;

                k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
                k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
                k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
                k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

                MeshWriteData mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length);
                mwd.SetAllVertices(k_Vertices);
                mwd.SetAllIndices(k_Indices);
            }
        }

        class SolidHexagon : VisualElement
        {
            static readonly Vertex[] k_Vertices = new Vertex[7];
            static readonly ushort[] k_Indices =
            {
                0, 2, 1,
                0, 3, 2,
                0, 4, 3,
                0, 5, 4,
                0, 6, 5,
                0, 1, 6
            };

            static SolidHexagon()
            {
                k_Vertices[0].tint = Color.white;
                k_Vertices[1].tint = Color.red;
                k_Vertices[2].tint = Color.yellow;
                k_Vertices[3].tint = Color.green;
                k_Vertices[4].tint = Color.cyan;
                k_Vertices[5].tint = Color.blue;
                k_Vertices[6].tint = Color.magenta;
            }

            public SolidHexagon()
            {
                generateVisualContent += OnGenerateVisualContent;
            }

            void OnGenerateVisualContent(MeshGenerationContext mgc)
            {
                Rect r = contentRect;
                if (r.width < 0.01f || r.height < 0.01f)
                    return; // Skip rendering when too small.

                float radiusX = r.width / 2;
                float radiusY = r.height / 2;

                k_Vertices[0].position = new Vector3(radiusX, radiusY, Vertex.nearZ);

                float angle = 0;
                for (int i = 1; i < 7; ++i)
                {
                    k_Vertices[i].position =  new Vector3(
                        radiusX + radiusX * Mathf.Cos(angle),
                        radiusY - radiusY * Mathf.Sin(angle),
                        Vertex.nearZ);
                    angle += 2f * Mathf.PI / 6;
                }

                MeshWriteData mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length);
                mwd.SetAllVertices(k_Vertices);
                mwd.SetAllIndices(k_Indices);
            }
        }

        class TexturedCheckerboard : VisualElement
        {
            static readonly Vertex[] k_Vertices = new Vertex[4];
            static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };

            static TexturedCheckerboard()
            {
                k_Vertices[0].tint = Color.white;
                k_Vertices[1].tint = Color.white;
                k_Vertices[2].tint = Color.white;
                k_Vertices[3].tint = Color.white;
            }

            public TexturedCheckerboard()
            {
                generateVisualContent += OnGenerateVisualContent;
                m_Texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("edb91687c58d28f4cbec9a58f96ab223"));
            }

            Texture2D m_Texture;

            void OnGenerateVisualContent(MeshGenerationContext mgc)
            {
                Rect r = contentRect;
                if (r.width < 0.01f || r.height < 0.01f)
                    return; // Skip rendering when too small.

                float left = 0;
                float right = r.width;
                float top = 0;
                float bottom = r.height;

                k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
                k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
                k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
                k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

                MeshWriteData mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length, m_Texture);

                // Remap 0..1 to the uv region.
                Rect uvs = mwd.uvRegion;
                k_Vertices[0].uv = new Vector2(uvs.xMin, uvs.yMin);
                k_Vertices[1].uv = new Vector2(uvs.xMin, uvs.yMax);
                k_Vertices[2].uv = new Vector2(uvs.xMax, uvs.yMax);
                k_Vertices[3].uv = new Vector2(uvs.xMax, uvs.yMin);

                mwd.SetAllVertices(k_Vertices);
                mwd.SetAllIndices(k_Indices);
            }
        }
    }
}
