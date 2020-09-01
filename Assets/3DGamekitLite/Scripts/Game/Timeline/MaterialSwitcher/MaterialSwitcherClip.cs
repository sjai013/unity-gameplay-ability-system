using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MaterialSwitcherClip : PlayableAsset, ITimelineClipAsset
{
    public MaterialSwitcherBehaviour template = new MaterialSwitcherBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Extrapolation; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<MaterialSwitcherBehaviour>.Create (graph, template);
    }
}
