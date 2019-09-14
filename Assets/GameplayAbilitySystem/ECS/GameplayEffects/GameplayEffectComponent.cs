using Unity.Collections;
using Unity.Entities;

public struct GameplayEffectDurationComponent : IGameplayEffectComponent, IComponentData {
    public float WorldStartTime;
    public float Duration;
    public float TimeRemaining;

    public EGameplayEffect Effect;
}


public struct AttributeModificationComponent : IComponentData {
    public float Change;
    public float Multiply;
    public float Divide;
    public float Add;
    public Entity Source;
    public Entity Target;
}


public interface AttributeModification : IComponentData { }

public struct GameplayEffectExpired : IComponentData { }

public interface IGameplayEffectComponent  { }

public struct AttributeModificationUndoAppliedComponent : IComponentData { }

public struct MaxHealthAttributeModifier : IComponentData { }
public struct MaxManaAttributeModifier : IComponentData { }
