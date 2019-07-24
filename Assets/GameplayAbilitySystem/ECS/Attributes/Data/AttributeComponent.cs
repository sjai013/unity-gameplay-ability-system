using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

public struct BaseAttributeComponent : IComponentData {
    public float BaseValue;
    public float CurrentValue;
}

public interface IAttributeComponentData : IComponentData {
}

public partial struct AttributesComponent : IAttributeComponentData {
}