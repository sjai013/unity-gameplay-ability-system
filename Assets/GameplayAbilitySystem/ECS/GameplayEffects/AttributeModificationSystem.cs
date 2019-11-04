using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public struct TemporaryAttributeModification : IComponentData { }
public struct PermanentAttributeModification : IComponentData { }

