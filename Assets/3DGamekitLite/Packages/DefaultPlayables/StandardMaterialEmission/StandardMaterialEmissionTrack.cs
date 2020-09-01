using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.08920846f, 0.5514706f, 0.3793178f)]
[TrackClipType(typeof(StandardMaterialEmissionClip))]
[TrackBindingType(typeof(Renderer))]
public class StandardMaterialEmissionTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<StandardMaterialEmissionMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        Renderer trackBinding = director.GetGenericBinding (this) as Renderer;
        if (trackBinding == null)
            return;
            
        var serializedObject = new UnityEditor.SerializedObject (trackBinding);
        var iterator = serializedObject.GetIterator();
        
        while (iterator.NextVisible(true))
        {
            if (iterator.hasVisibleChildren)
                continue;
            
            driver.AddFromName<Renderer>(trackBinding.gameObject, iterator.propertyPath);
        }
#endif
        base.GatherProperties (director, driver);
    }
}
