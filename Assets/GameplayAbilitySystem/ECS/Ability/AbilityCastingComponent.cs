using System;
using Unity.Collections;
using Unity.Entities;


public struct Targetter : IComponentData {}
//public struct AbilityComponent : IComponentData {
//    public Entity Target;
//    public Entity Source;
//    public int AbilityId;
//}

[InternalBufferCapacity(32)]
public struct Targets : IBufferElementData {
    public Entity Target;
}