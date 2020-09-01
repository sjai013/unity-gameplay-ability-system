using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
[TrackClipType(typeof(TimeMachineClip))]
public class TimeMachineTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
		var scriptPlayable = ScriptPlayable<TimeMachineMixerBehaviour>.Create(graph, inputCount);

		TimeMachineMixerBehaviour b = scriptPlayable.GetBehaviour();
		b.markerClips = new System.Collections.Generic.Dictionary<string, double>();


		foreach (var c in GetClips())
		{
			TimeMachineClip clip = (TimeMachineClip)c.asset;
			string clipName = c.displayName;

			switch(clip.action)
			{
				case TimeMachineBehaviour.TimeMachineAction.Pause:
					clipName = "||";
					break;

				case TimeMachineBehaviour.TimeMachineAction.Marker:
					clipName = "● " + clip.markerLabel.ToString();

					//Insert the marker clip into the Dictionary of markers
					if(!b.markerClips.ContainsKey(clip.markerLabel)) //happens when you duplicate a clip and it has the same markerLabel
					{
						b.markerClips.Add(clip.markerLabel, (double)c.start);
					}
					break;

				case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
					clipName = "↩︎  " + clip.markerToJumpTo.ToString();
					break;

				case TimeMachineBehaviour.TimeMachineAction.JumpToTime:
					clipName = "↩ " + clip.timeToJumpTo.ToString();
					break;
			}

			c.displayName = clipName;


			if(clip.action == TimeMachineBehaviour.TimeMachineAction.Marker)
			{
				if(!b.markerClips.ContainsKey(clip.markerLabel)) //happens when you duplicate a clip and it has the same markerLabel
				{
					b.markerClips.Add(clip.markerLabel, (double)c.start);
				}
			}

		}

        return scriptPlayable;
    }


}
