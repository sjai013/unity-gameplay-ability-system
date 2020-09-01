using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D.SimpleSFX
{
    /// <summary>
    /// Generated simple loops using layers of oscillators which can have volume, frequency, and filter value envelopes.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SimpleFXSynth : MonoBehaviour
    {
        /// <summary>
        /// Components which share this name, will share the same audioclip, even when settings are different!
        /// </summary>
        public string fxName;
        public float duration = 5;
        public bool playOnStart = true;
        public SynthLayer[] layers;
        new public AudioSource audio;

        static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

        void Reset()
        {
            audio = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            Init();
        }

        IEnumerator Start()
        {
            if (playOnStart)
            {
                yield return new WaitForSeconds(Random.value * 2);
                Play();
            }
        }

        void Init()
        {
            if (audio == null) audio = GetComponent<AudioSource>();
            if (audio == null) return;
            AudioClip clip;
            if (Application.isPlaying && clips.ContainsKey(fxName))
                clip = clips[fxName];
            else
            {
                clip = clips[fxName] = AudioClip.Create(this.name, Mathf.FloorToInt(44100 * duration), 2, 44100, false);
                clip.SetData(RenderAudio(), 0);
            }
            audio.clip = clip;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            Init();
            audio.Play();

        }

        float[] RenderAudio()
        {
            foreach (var layer in layers)
                layer.Reset();
            var channels = 2;
            var data = new float[Mathf.FloorToInt((duration * 44100) * channels)];
            for (var i = 0; i < data.Length; i += channels)
            {
                var smp = 0f;
                foreach (var layer in layers)
                {
                    smp += layer.Sample(duration);
                }
                data[i + 0] = smp;
                data[i + 1] = smp;
            }
            return data;
        }


    }
}