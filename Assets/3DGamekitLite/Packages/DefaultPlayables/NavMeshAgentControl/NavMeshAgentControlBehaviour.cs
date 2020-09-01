using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class NavMeshAgentControlBehaviour : PlayableBehaviour
{
    public Transform destination;
    public bool destinationSet;

    public override void OnGraphStart (Playable playable)
    {
        destinationSet = false;
    }
}
