using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SetLocationMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;

        if (trackBinding == null)
            return;
        
        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<SetLocationBehaviour> inputPlayable = (ScriptPlayable<SetLocationBehaviour>)playable.GetInput(i);
            SetLocationBehaviour input = inputPlayable.GetBehaviour ();

            if (Mathf.Approximately (inputWeight, 1f))
            {
                trackBinding.position = input.position;
                trackBinding.eulerAngles = input.eulerAngles;
            }
        }
    }
}
