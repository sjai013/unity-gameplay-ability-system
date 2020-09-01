using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeDilationMixerBehaviour : PlayableBehaviour
{
    float m_OldTimeScale = 1f;

    public override void OnPlayableCreate (Playable playable)
    {
        m_OldTimeScale = Time.timeScale;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount ();

        float mixedTimeScale = 0f;
        float totalWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            totalWeight += inputWeight;

            ScriptPlayable<TimeDilationBehaviour> playableInput = (ScriptPlayable<TimeDilationBehaviour>)playable.GetInput (i);
            TimeDilationBehaviour input = playableInput.GetBehaviour ();

            mixedTimeScale += inputWeight * input.timeScale;
        }

        Time.timeScale = mixedTimeScale + m_OldTimeScale * (1f - totalWeight);
    }

    public override void OnGraphStop (Playable playable)
    {
        Time.timeScale = m_OldTimeScale;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        Time.timeScale = m_OldTimeScale;
    }
}
