using Unity.Collections;
using Unity.Entities;

public struct GameplayEffectDurationComponent : IComponentData {
    public float WorldStartTime;
    public float Duration;
    public Entity Source;
}


public struct GameplayEffectAttributeModificationComponent : IComponentData {
    public float AttributeChangeBase;
    public float AttributeChangeCurrent;
    public Entity Target;
    public Entity Source;

    // How to identify attribute that is to be changed?

}

public struct GameplayEffectExpired : IComponentData { }
