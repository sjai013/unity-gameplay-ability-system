using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class SoundTrack : MonoBehaviour
    {
        public float soundTrackVolume = 1;
        public float initialVolume = 1;
        public float volumeRampSpeed = 4;
        public bool playOnStart = true;
        public AudioSource[] audioSources;

        AudioSource activeAudio, fadeAudio;
        float volumeVelocity, fadeVelocity;
        float volume;
        Stack<string> trackStack = new Stack<string>();

        public void PushTrack(string name)
        {
            trackStack.Push(name);
            Enqueue(name);
        }

        public void PopTrack()
        {
            if (trackStack.Count > 1)
                trackStack.Pop();
            Enqueue(trackStack.Peek());
        }

        public void Enqueue(string name)
        {
            foreach (var i in audioSources)
            {
                if (i.name == name)
                {
                    fadeAudio = activeAudio;
                    activeAudio = i;
                    if (!activeAudio.isPlaying) activeAudio.Play();
                    break;
                }
            }
        }

        public void Play()
        {
            if (activeAudio != null)
                activeAudio.Play();
        }

        public void Stop()
        {
            foreach (var i in audioSources) i.Stop();
        }

        void OnEnable()
        {
            trackStack.Clear();
            if (audioSources.Length > 0)
            {
                activeAudio = audioSources[0];
                foreach (var i in audioSources) i.volume = 0;
                trackStack.Push(audioSources[0].name);
                if (playOnStart) Play();
            }
            volume = initialVolume;
        }

        void Reset()
        {
            audioSources = GetComponentsInChildren<AudioSource>();
        }

        public void SetVolume(float volume)
        {
            this.volume = volume;
        }

        void Update()
        {
            if (activeAudio != null)
                activeAudio.volume = Mathf.SmoothDamp(activeAudio.volume, volume * soundTrackVolume, ref volumeVelocity, volumeRampSpeed, 1);

            if (fadeAudio != null)
            {
                fadeAudio.volume = Mathf.SmoothDamp(fadeAudio.volume, 0, ref fadeVelocity, volumeRampSpeed, 1);
                if (Mathf.Approximately(fadeAudio.volume, 0))
                {
                    fadeAudio.Stop();
                    fadeAudio = null;
                }
            }
        }
    }
}
