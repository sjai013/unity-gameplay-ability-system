using Unity.Entities;
/// <summary>
/// Provides collection of functionality all game effects need to have
/// </summary>
public interface IGameplayEffect {
    Entity Target { get; set; }
    Entity Source { get; set; }
    void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime);
    void ApplyGameplayEffect(EntityManager EntityManager, AttributesComponent attributesComponent, float WorldTime);
    DurationPolicyComponent DurationPolicy { get; set; }
}

public interface IPeriodicEffect : IGameplayEffect {
    void CreatePeriodicEffectEntity(int index, EntityCommandBuffer.Concurrent Ecb, AttributesComponent attributesComponent, float WorldTime);
}