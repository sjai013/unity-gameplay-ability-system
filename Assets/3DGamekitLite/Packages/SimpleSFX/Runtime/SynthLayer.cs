using UnityEngine;

namespace Gamekit3D.SimpleSFX
{
    [System.Serializable]
    public class SynthLayer
    {
        public OscType oscType;
        public AnimationCurve oscillatorFrequency = AnimationCurve.Linear(0, 440, 1, 440);
        public AnimationCurve volumeEnvelope = AnimationCurve.Linear(0, 1, 0, 1);
        public FilterType filterType;
        public AnimationCurve cutoffEnvelope = AnimationCurve.Linear(0, 1, 0, 1);
        public AnimationCurve resonanceEnvelope = AnimationCurve.Linear(0, 1, 0, 1);
        [System.NonSerialized] public float phase = 0f;
        [System.NonSerialized] public float time = 0f;
        [System.NonSerialized] BQFilter bqFilter = new BQFilter();

        const float TWOPI = Mathf.PI * 2;
        const int SAMPLERATE = 44100;

        public void Reset()
        {
            phase = 0f;
            time = 0f;
        }

        float SampleOsc()
        {
            switch (oscType)
            {
                case OscType.Sin:
                    return BandLimit(Mathf.Sin(phase));
                case OscType.Square:
                    return BandLimit(phase < Mathf.PI ? 1f : -1f);
                case OscType.PWM:
                    return BandLimit(phase < Mathf.PI * 0.25f ? 1f : -1f);
                case OscType.Tan:
                    return BandLimit(Mathf.Clamp(Mathf.Tan(phase / 2), -1, 1));
                case OscType.Saw:
                    return BandLimit(1f - (1f / Mathf.PI * phase));
                case OscType.Triangle:
                    if (phase < Mathf.PI)
                        return BandLimit(-1f + (2 * 1f / Mathf.PI) * phase);
                    else
                        return BandLimit(3f - (2 * 1f / Mathf.PI) * phase);
                case OscType.Random:
                    return Random.value * 2 - 1;
                default:
                    return 0;
            }
        }

        float BandLimit(float smp)
        {
            //This is a LPF at 22049hz.
            xv[0] = xv[1];
            xv[1] = smp / 1.000071238f;
            yv[0] = yv[1];
            yv[1] = (xv[0] + xv[1]) + (-0.9998575343f * yv[0]);
            return yv[1];
        }

        float[] xv = new float[2], yv = new float[2];

        public float Sample(float duration)
        {
            var smp = SampleOsc();
            time += 1f / SAMPLERATE;
            var a = volumeEnvelope.Evaluate(time / duration);
            var c = cutoffEnvelope.Evaluate(time / duration);
            var r = resonanceEnvelope.Evaluate(time / duration);
            smp *= a;
            phase = phase + ((TWOPI * oscillatorFrequency.Evaluate(phase)) / SAMPLERATE);
            if (phase > TWOPI)
                phase -= TWOPI;

            switch (filterType)
            {
                case FilterType.Lowpass:
                    bqFilter.SetLowPass(c, r);
                    smp = bqFilter.Update(smp);
                    break;
                case FilterType.Highpass:
                    bqFilter.SetHighPass(c, r);
                    smp = bqFilter.Update(smp);
                    break;
                case FilterType.Bandpass:
                    bqFilter.SetBandPass(c, r);
                    smp = bqFilter.Update(smp);
                    break;
                case FilterType.Bandstop:
                    bqFilter.SetBandStop(c, r);
                    smp = bqFilter.Update(smp);
                    break;
                case FilterType.Allpass:
                    bqFilter.SetAllPass(c, r);
                    smp = bqFilter.Update(smp);
                    break;
            }

            return smp;
        }

    }
}