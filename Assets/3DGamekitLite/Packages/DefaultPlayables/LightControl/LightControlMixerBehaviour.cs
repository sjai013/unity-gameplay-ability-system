using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class LightControlMixerBehaviour : PlayableBehaviour
{
    Color m_DefaultColor;
    float m_DefaultIntensity;
    float m_DefaultBounceIntensity;
    float m_DefaultRange;

    Light m_TrackBinding;
    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as Light;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened)
        {
            m_DefaultColor = m_TrackBinding.color;
            m_DefaultIntensity = m_TrackBinding.intensity;
            m_DefaultBounceIntensity = m_TrackBinding.bounceIntensity;
            m_DefaultRange = m_TrackBinding.range;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount ();

        Color blendedColor = Color.clear;
        float blendedIntensity = 0f;
        float blendedBounceIntensity = 0f;
        float blendedRange = 0f;
        float totalWeight = 0f;
        float greatestWeight = 0f;
        int currentInputs = 0;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<LightControlBehaviour> inputPlayable = (ScriptPlayable<LightControlBehaviour>)playable.GetInput(i);
            LightControlBehaviour input = inputPlayable.GetBehaviour ();
            
            blendedColor += input.color * inputWeight;
            blendedIntensity += input.intensity * inputWeight;
            blendedBounceIntensity += input.bounceIntensity * inputWeight;
            blendedRange += input.range * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
            }

            if (!Mathf.Approximately (inputWeight, 0f))
                currentInputs++;
        }

        m_TrackBinding.color = blendedColor + m_DefaultColor * (1f - totalWeight);
        m_TrackBinding.intensity = blendedIntensity + m_DefaultIntensity * (1f - totalWeight);
        m_TrackBinding.bounceIntensity = blendedBounceIntensity + m_DefaultBounceIntensity * (1f - totalWeight);
        m_TrackBinding.range = blendedRange + m_DefaultRange * (1f - totalWeight);
        if (currentInputs != 1 && 1f - totalWeight > greatestWeight)
        {
        }
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        m_TrackBinding.color = m_DefaultColor;
        m_TrackBinding.intensity = m_DefaultIntensity;
        m_TrackBinding.bounceIntensity = m_DefaultBounceIntensity;
        m_TrackBinding.range = m_DefaultRange;
        m_FirstFrameHappened = false;
    }
}
