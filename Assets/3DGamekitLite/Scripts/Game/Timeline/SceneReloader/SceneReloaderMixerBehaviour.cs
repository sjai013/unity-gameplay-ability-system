using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SceneReloaderMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        GameObject trackBinding = playerData as GameObject;

        if(trackBinding == null)
            return;

        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<SceneReloaderBehaviour> inputPlayable = (ScriptPlayable<SceneReloaderBehaviour>)playable.GetInput(i);
            SceneReloaderBehaviour input = inputPlayable.GetBehaviour ();

            if (Mathf.Approximately (inputWeight, 1f) && Application.isPlaying)
            {
                input.ReloadScene (trackBinding);
            }
        }
    }
}
