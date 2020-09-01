using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class CaptureDepthTexture : MonoBehaviour
    {
        public Shader depthOnlyShader;

        protected List<Camera> _cameraBufferAdded = new List<Camera>();

        CommandBuffer cb = null;
        private Material m = null;
        //private RenderTexture RT;

        void OnEnable()
        {
            CreateBuffer();
            _cameraBufferAdded.Clear();

            Camera.onPreRender += PreRenderCamera;
        }

        void CreateBuffer()
        {
            if (depthOnlyShader != null)
            {
                cb = new CommandBuffer();
                cb.name = "Render Water Separate Depth";

                cb.GetTemporaryRT(Shader.PropertyToID("WaterDepthTemp"), new RenderTextureDescriptor(-1, -1, RenderTextureFormat.Depth, 24), FilterMode.Point);
                cb.SetRenderTarget(new RenderTargetIdentifier("WaterDepthTemp"));
                cb.ClearRenderTarget(true, true, Color.white);

                m = new Material(depthOnlyShader);
                var renderer = GetComponent<MeshRenderer>();
                cb.DrawRenderer(renderer, m);

                cb.SetGlobalTexture("_WaterDepthMap", new RenderTargetIdentifier("WaterDepthTemp"));
            }
        }

        void PreRenderCamera(Camera cam)
        {
            if(cb == null)
             CreateBuffer();

            if (cb == null)
                return;

            if ((cam.cullingMask & (1 << gameObject.layer)) == 0)
                return;
            
            if (!_cameraBufferAdded.Contains(cam))
            {
                _cameraBufferAdded.Add(cam);
                cam.AddCommandBuffer(CameraEvent.AfterGBuffer, cb);
            }
        }

        private void OnDisable()
        {
            Camera.onPreRender -= PreRenderCamera;

            for (int i = 0; i < _cameraBufferAdded.Count; ++i)
            {
                if (_cameraBufferAdded[i] == null)
                    return;

                _cameraBufferAdded[i].RemoveCommandBuffer(CameraEvent.AfterGBuffer, cb);
            }

            if (m != null)
            {
                DestroyImmediate(m);
            }
        }
    }
}