using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.7794118f, 0.02865483f, 0.02865483f)]
[TrackClipType(typeof(SceneReloaderClip))]
[TrackBindingType(typeof(GameObject))]
public class SceneReloaderTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SceneReloaderMixerBehaviour>.Create (graph, inputCount);
    }
}
