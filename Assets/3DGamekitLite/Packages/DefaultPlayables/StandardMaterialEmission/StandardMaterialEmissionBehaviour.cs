using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class StandardMaterialEmissionBehaviour : PlayableBehaviour
{
    public int materialIndex;
    [ColorUsage(false, true)]
    public Color color;
    public bool materialIndicesMatch = true;
}
