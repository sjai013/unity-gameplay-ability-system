using Unity.Entities;
/// <summary>
/// Tag to indicate the entity is currently casting, and therefore not able to cast.  
/// Useful for restricting to only 1 cast per frame
/// </summary>
struct CastingAbilityTagComponent : IComponentData { }
