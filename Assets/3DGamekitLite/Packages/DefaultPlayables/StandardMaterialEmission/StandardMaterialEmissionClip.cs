using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class StandardMaterialEmissionClip : PlayableAsset, ITimelineClipAsset
{
    public StandardMaterialEmissionBehaviour template = new StandardMaterialEmissionBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.All; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<StandardMaterialEmissionBehaviour>.Create (graph, template);
        return playable;
    }
}
