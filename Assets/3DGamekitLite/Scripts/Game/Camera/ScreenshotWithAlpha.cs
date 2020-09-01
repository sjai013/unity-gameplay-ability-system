using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Gamekit3D
{
    [RequireComponent(typeof(Camera))]
    public class ScreenshotWithAlpha : MonoBehaviour
    {
        public int UpScale = 4;
        public bool AlphaBackground = true;

        Texture2D Screenshot()
        {
            var camera = GetComponent<Camera>();
            int w = camera.pixelWidth * UpScale;
            int h = camera.pixelHeight * UpScale;
            var rt = new RenderTexture(w, h, 32);
            camera.targetTexture = rt;
            var screenShot = new Texture2D(w, h, TextureFormat.ARGB32, false);
            var clearFlags = camera.clearFlags;
            if (AlphaBackground)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0, 0, 0, 0);
            }
            var cameras = new List<Camera>(FindObjectsOfType<Camera>());
            cameras.Sort((A, B) => A.depth.CompareTo(B.depth));
            foreach (var c in cameras) c.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            var pixels = screenShot.GetPixels();

            for (int i = 0; i < pixels.Length; ++i)
                pixels[i].a = 1;

            screenShot.SetPixels(pixels);
            screenShot.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);
            camera.clearFlags = clearFlags;
            return screenShot;
        }

        [ContextMenu("Capture Child Positions")]
        public void SaveChildScreenshots()
        {
            StartCoroutine(_SaveChildScreenshots());
        }

        IEnumerator<YieldInstruction> _SaveChildScreenshots()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            {
                var originP = transform.position;
                var originR = transform.rotation;
                for (var i = 0; i < transform.childCount; i++)
                {
                    var tx = transform.GetChild(i);
                    transform.position = tx.position;
                    transform.rotation = tx.rotation;
                    yield return new WaitForSeconds(1);
                    var filename = "SS-Group-" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".png";
                    File.WriteAllBytes(Path.Combine(path, filename), Screenshot().EncodeToPNG());
                    transform.position = originP;
                    transform.rotation = originR;
                }
            }
        }

        [ContextMenu("Capture Screenshot Custom")]
        public void SaveScreenshot()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filename = "SS-Custom-" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".png";
            File.WriteAllBytes(Path.Combine(path, filename), Screenshot().EncodeToPNG());
        }

        [ContextMenu("Capture Screenshot Builtin")]
        public void SaveScreenshotBuiltin()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var filename = "SS-Builtin-" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".png";
            ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(path, filename), UpScale);
        }
    }
}