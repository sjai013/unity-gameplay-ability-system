using Unity.Collections;
using Unity.Entities;

public struct GameplayEffectDurationComponent : IComponentData {
    public float WorldStartTime;
    public float Duration;
}

public struct GameplayEffectAttributeModificationComponent : IComponentData {
    //public NativeArray<float> Multiplier;
    //public NativeArray<float> Divider;
    //public NativeArray<float> Adder;
}
