using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneScriptControlMixerBehaviour : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if(!Mathf.Approximately (inputWeight, 1f))
                continue;

            ScriptPlayable<CutsceneScriptControlBehaviour> inputPlayable = (ScriptPlayable<CutsceneScriptControlBehaviour>)playable.GetInput(i);
            CutsceneScriptControlBehaviour input = inputPlayable.GetBehaviour ();

            input.playerInput.enabled = input.playerInputEnabled;
        }
    }
}
