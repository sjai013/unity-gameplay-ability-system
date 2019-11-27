using GameplayAbilitySystem;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct MouseDelta : IComponentData {
    public float x;
    public float y;
}