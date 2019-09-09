using Unity.Entities;
/// <summary>
/// Provides collection of functionality that cooldowns need to have
/// </summary>
public interface ICooldown {
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Caster, float WorldTime);
    EGameplayEffect GameplayEffect { get; }
}
