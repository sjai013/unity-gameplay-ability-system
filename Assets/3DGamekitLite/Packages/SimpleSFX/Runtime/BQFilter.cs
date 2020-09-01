using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gamekit3D.SimpleSFX
{
    // Based on naudio's BiQuadFilter by Mark Heath
    // https://github.com/naudio/NAudio/blob/master/NAudio/Dsp/BiQuadFilter.cs
    // License: MSPL

    public class BQFilter
    {
        const int sampleRate = 44100;

        float a0, a1, a2, a3, a4;
        float x1 = 0, x2 = 0, y1 = 0, y2 = 0;


        public float Update(float v)
        {
            var smp = a0 * v + a1 * x1 + a2 * x2 - a3 * y1 - a4 * y2;
            x2 = float.IsNaN(x1) ? 0 : x1;
            x1 = float.IsNaN(v) ? 0 : v;
            y2 = float.IsNaN(y1) ? 0 : y1;
            y1 = float.IsNaN(smp) ? 0 : smp;
            return y1;
        }


        void SetCoeff(float aa0, float aa1, float aa2, float b0, float b1, float b2)
        {
            a0 = b0 / aa0;
            a1 = b1 / aa0;
            a2 = b2 / aa0;
            a3 = aa1 / aa0;
            a4 = aa2 / aa0;
        }


        public void SetLowPass(float freq, float q)
        {
            freq = Mathf.Clamp(freq, Mathf.Epsilon, sampleRate / 4);
            if (q <= 0.01f) q = 0.01f;
            var w0 = 2 * Mathf.PI * freq / sampleRate;
            var cosw0 = Mathf.Cos(w0);
            var alpha = Mathf.Sin(w0) / (2 * q);
            var b0 = (1 - cosw0) / 2;
            var b1 = 1 - cosw0;
            var b2 = (1 - cosw0) / 2;
            var aa0 = 1 + alpha;
            var aa1 = -2 * cosw0;
            var aa2 = 1 - alpha;
            SetCoeff(aa0, aa1, aa2, b0, b1, b2);
        }


        public void SetHighPass(float freq, float q)
        {
            freq = Mathf.Clamp(freq, Mathf.Epsilon, sampleRate / 4);
            if (q <= 0.01f) q = 0.01f;
            var w0 = 2 * Mathf.PI * freq / sampleRate;
            var cosw0 = Mathf.Cos(w0);
            var alpha = Mathf.Sin(w0) / (2 * q);

            var b0 = (1 + cosw0) / 2;
            var b1 = -(1 + cosw0);
            var b2 = (1 + cosw0) / 2;
            var aa0 = 1 + alpha;
            var aa1 = -2 * cosw0;
            var aa2 = 1 - alpha;
            SetCoeff(aa0, aa1, aa2, b0, b1, b2);
        }


        public void SetBandPass(float freq, float q)
        {
            freq = Mathf.Clamp(freq, Mathf.Epsilon, sampleRate / 4);
            if (q <= 0.01f) q = 0.01f;
            var w0 = 2 * Mathf.PI * freq / sampleRate;
            var cosw0 = Mathf.Cos(w0);
            var sinw0 = Mathf.Sin(w0);
            var alpha = sinw0 / (2 * q);
            var b0 = alpha;
            var b1 = 0;
            var b2 = -alpha;
            var a0 = 1 + alpha;
            var a1 = -2 * cosw0;
            var a2 = 1 - alpha;
            SetCoeff(a0, a1, a2, b0, b1, b2);
        }


        public void SetBandStop(float freq, float q)
        {
            freq = Mathf.Clamp(freq, Mathf.Epsilon, sampleRate / 4);
            if (q <= 0.01f) q = 0.01f;
            var w0 = 2 * Mathf.PI * freq / sampleRate;
            var cosw0 = Mathf.Cos(w0);
            var sinw0 = Mathf.Sin(w0);
            var alpha = sinw0 / (2 * q);

            var b0 = 1;
            var b1 = -2 * cosw0;
            var b2 = 1;
            var a0 = 1 + alpha;
            var a1 = -2 * cosw0;
            var a2 = 1 - alpha;
            SetCoeff(a0, a1, a2, b0, b1, b2);
        }


        public void SetAllPass(float freq, float q)
        {
            //H(s) = (s^2 - s/Q + 1) / (s^2 + s/Q + 1)
            var w0 = 2 * Mathf.PI * freq / sampleRate;
            var cosw0 = Mathf.Cos(w0);
            var sinw0 = Mathf.Sin(w0);
            var alpha = sinw0 / (2 * q);

            var b0 = 1 - alpha;
            var b1 = -2 * cosw0;
            var b2 = 1 + alpha;
            var a0 = 1 + alpha;
            var a1 = -2 * cosw0;
            var a2 = 1 - alpha;
            SetCoeff(a0, a1, a2, b0, b1, b2);
        }

    }

}