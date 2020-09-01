using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SetLocationClip : PlayableAsset, ITimelineClipAsset
{
    public SetLocationBehaviour template = new SetLocationBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SetLocationBehaviour>.Create (graph, template);
        return playable;
    }
}
