using Unity.Entities;
/// <summary>
/// Provides collection of functionality that cooldowns need to have
/// </summary>
public interface ICooldown {
    Entity Caster { get; set; }
    void ApplyCooldownEffect(int index, EntityCommandBuffer.Concurrent Ecb, float WorldTime);
    EGameplayEffect GameplayEffect { get; }
}
