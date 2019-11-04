using Unity.Collections;
using Unity.Entities;

public struct _GameplayEffectDurationComponent : _IGameplayEffectComponent, IComponentData {
    public float WorldStartTime;
    public float Duration;
    public float TimeRemaining;

    public EGameplayEffect Effect;
}


public struct _AttributeModificationComponent : IComponentData {
    public float Change;
    public float Multiply;
    public float Divide;
    public float Add;
    public Entity Source;
    public Entity Target;
}


public interface _AttributeModification : IComponentData { }

public struct _GameplayEffectExpired : IComponentData { }

public interface _IGameplayEffectComponent  { }

public struct _AttributeModificationUndoAppliedComponent : IComponentData { }

public struct _MaxHealthAttributeModifier : IComponentData { }
public struct _MaxManaAttributeModifier : IComponentData { }
