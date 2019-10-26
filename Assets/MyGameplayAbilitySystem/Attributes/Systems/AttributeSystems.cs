using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.Systems;
namespace MyGameplayAbilitySystem.Attributes.Systems {
    public class HealthAttributeSystem : GenericAttributeSystem<HealthAttributeComponent> { }
    public class ManaAttributeSystem : GenericAttributeSystem<ManaAttributeComponent> { }
    public class MaxHealthAttributeSystem : GenericAttributeSystem<MaxHealthAttributeComponent> { }
    public class MaxManaAttributeSystem : GenericAttributeSystem<MaxManaAttributeComponent> { }
    public class CharacterLevelAttributeSystem : GenericAttributeSystem<CharacterLevelAttributeComponent> { }


}