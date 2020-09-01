using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class AudioSnapshotBehaviour : PlayableBehaviour
{
    public enum AudioPlayMode
    {
        PlayAudio, DontPlayAudio, PlayAudioIfNotPlaying,
    }

    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioMixerSnapshot snapshot;
    public float volume;
    public bool weightedVolume;
    public AudioPlayMode audioPlayMode;
    
    bool m_IsPlaying;

    public void PlayAudio (float weight)
    {
        if(audioPlayMode == AudioPlayMode.DontPlayAudio || (audioPlayMode == AudioPlayMode.PlayAudioIfNotPlaying && audioSource.isPlaying))
            return;

        if(audioSource == null)
            return;

        if(audioClip == null || audioSource.clip == null)
            return;

        if (weight > float.Epsilon)
        {
            if (!m_IsPlaying)
            {
                if (audioSource.clip != audioClip || !audioSource.isPlaying)
                {
                    if(audioClip != null)
                        audioSource.clip = audioClip;

                    audioSource.Play();
                }
            }

            m_IsPlaying = true;
        }
        else
        {
            m_IsPlaying = false;
        }
    }
}
