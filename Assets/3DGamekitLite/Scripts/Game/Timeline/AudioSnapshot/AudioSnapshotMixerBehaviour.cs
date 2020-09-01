using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AudioSnapshotMixerBehaviour : PlayableBehaviour
{
    AudioMixer m_Mixer;
    AudioMixerSnapshot[] m_Snapshots;
    float[] m_CurrentWeights;

    public override void OnGraphStart (Playable playable)
    {
        int inputCount = playable.GetInputCount ();

        m_Snapshots = new AudioMixerSnapshot[inputCount];
        m_CurrentWeights = new float[inputCount];

        for (int i = 0; i < inputCount; i++)
        {
            ScriptPlayable<AudioSnapshotBehaviour> inputPlayable = (ScriptPlayable<AudioSnapshotBehaviour>)playable.GetInput(i);
            AudioSnapshotBehaviour input = inputPlayable.GetBehaviour ();

            m_Snapshots[i] = input.snapshot;
        }

        if(m_Snapshots.Length > 0)
            m_Mixer = m_Snapshots[0].audioMixer;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // First frame of each behaviour: play audio clip if given, play audio source if given
        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            m_CurrentWeights[i] = inputWeight;

            ScriptPlayable<AudioSnapshotBehaviour> inputPlayable = (ScriptPlayable<AudioSnapshotBehaviour>)playable.GetInput(i);
            AudioSnapshotBehaviour input = inputPlayable.GetBehaviour();

            if(Application.isPlaying)
                input.PlayAudio (inputWeight);

            input.audioSource.volume = input.weightedVolume ? input.volume * playable.GetInputWeight (i) : input.volume;
        }

        if(m_Mixer != null)
            m_Mixer.TransitionToSnapshots(m_Snapshots, m_CurrentWeights, 0f);
    }
}
