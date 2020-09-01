using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Light))]
    public class ModulatedLight : MonoBehaviour
    {

        public enum ModulationType
        {
            Sine, Triangle, Perlin, Random
        }

        public bool executeInEditMode = true;
        public ModulationType type = ModulationType.Sine;
        public float frequency = 1f;
        public Color colorA = Color.red;
        public Color colorB = Color.blue;

        public new Light light;

        float TriangleWave(float x)
        {
            var frac = x - (int)x;
            var a = frac * 2.0f - 1.0f;
            return a > 0 ? a : -a;
        }

        void Reset()
        {
            light = GetComponent<Light>();
            if (light != null) colorA = light.color;
        }

        void Update()
        {
            if (light == null) light = GetComponent<Light>();
            var t = 0f;
            switch (type)
            {
                case ModulationType.Sine:
                    t = Mathf.Sin(Time.time * frequency);
                    break;
                case ModulationType.Triangle:
                    t = TriangleWave(Time.time * frequency);
                    break;
                case ModulationType.Perlin:
                    t = Mathf.PerlinNoise(Time.time * frequency, 0.5f);
                    break;
                case ModulationType.Random:
                    t = Random.value;
                    break;
            }
            light.color = Color.Lerp(colorA, colorB, t);
        }

    }

}
