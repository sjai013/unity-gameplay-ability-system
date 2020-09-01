using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    [ExecuteInEditMode]
    public class GlobalShaderSettings : MonoBehaviour
    {

        [SerializeField]
        float TopScale = 1;
        [SerializeField]
        float NormalDetailScale = 1;
        [SerializeField]
        float NoiseAmount = 1;
        [SerializeField]
        float NoiseFalloff = 1;
        [SerializeField]
        float NoiseScale = 1;
        [SerializeField]
        float FresnelAmount = 0.5f;
        [SerializeField]
        float FresnelPower = 0.5f;

        void Update()
        {
            Shader.SetGlobalFloat("_TopScale", TopScale);
            Shader.SetGlobalFloat("_TopNormal2Scale", NormalDetailScale);
            Shader.SetGlobalFloat("_NoiseAmount", NoiseAmount);
            Shader.SetGlobalFloat("_NoiseFallOff", NoiseFalloff);
            Shader.SetGlobalFloat("_noiseScale", NoiseScale);
            Shader.SetGlobalFloat("_FresnelAmount", FresnelAmount);
            Shader.SetGlobalFloat("_FresnelPower", FresnelPower);
        }
    } 
}
