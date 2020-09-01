using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class NavMeshAgentControlClip : PlayableAsset, ITimelineClipAsset
{
    public ExposedReference<Transform> destination;
    [HideInInspector]
    public NavMeshAgentControlBehaviour template = new NavMeshAgentControlBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<NavMeshAgentControlBehaviour>.Create (graph, template);
        NavMeshAgentControlBehaviour clone = playable.GetBehaviour ();
        clone.destination = destination.Resolve (graph.GetResolver ());
        return playable;
    }
}
