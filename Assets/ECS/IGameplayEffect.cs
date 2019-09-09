using Unity.Entities;
/// <summary>
/// Provides collection of functionality all game effects need to have
/// </summary>
public interface IGameplayEffect {
    void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent);
    void ApplyGameplayEffect(EntityManager EntityManager, Entity Source, Entity Target, AttributesComponent attributesComponent);

    DurationPolicyComponent DurationPolicy { get; set; }
}
