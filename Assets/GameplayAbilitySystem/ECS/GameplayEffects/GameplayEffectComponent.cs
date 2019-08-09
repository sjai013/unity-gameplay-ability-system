using Unity.Collections;
using Unity.Entities;

public struct GameplayEffectDurationComponent : GameplayEffectComponent, IComponentData {
    public float WorldStartTime;
    public float Duration;
    public float TimeRemaining;
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

public struct PeriodicGameplayEffect : IComponentData {
    public float Period;
    public Entity GameplayEffectToExecute;
}

public interface GameplayEffectComponent  { }

public struct AttributeModificationUndoAppliedComponent : IComponentData { }

public struct MaxHealthAttributeModifier : IComponentData { }
public struct MaxManaAttributeModifier : IComponentData { }
