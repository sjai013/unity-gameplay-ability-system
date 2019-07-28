using Unity.Collections;
using Unity.Entities;

public struct GameplayEffectDurationComponent : IComponentData {
    public float WorldStartTime;
    public float Duration;
    public float TimeRemaining;
}


public struct TemporaryAttributeModificationComponent : AttributeModificationComponent {
    public float Change;
    public Entity Source;
    public Entity Target;
    public Entity GameplayEffectDuration;
}

public struct PermanentAttributeModificationComponent : AttributeModificationComponent {
    public float Change;
    public Entity Source;
    public Entity Target;
}

public struct GameplayEffectExpired : IComponentData { }

public struct PeriodicGameplayEffect : IComponentData {
    public float Period;
    public Entity GameplayEffectToExecute;
}

public interface AttributeModificationComponent : IComponentData {

}

public struct AttributeModificationUndoAppliedComponent : IComponentData { }