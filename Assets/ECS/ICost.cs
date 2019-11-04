using Unity.Entities;
/// <summary>
/// Provides collection of functionality that cost effects need to have
/// </summary>
public interface ICost : _IGameplayEffect {
    AttributesComponent ComputeResourceUsage(Entity Caster, AttributesComponent attributes);
}
