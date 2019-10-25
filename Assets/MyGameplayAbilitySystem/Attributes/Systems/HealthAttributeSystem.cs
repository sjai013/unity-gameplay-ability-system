using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.Systems;
namespace MyGameplayAbilitySystem.Attributes.Systems {
    public class HealthAttributeSystem : GenericAttributeSystem<HealthAttributeComponent> { }
    public class ManaAttributeSystem : GenericAttributeSystem<ManaAttributeComponent> { }

    public class HealthAttributeSystem2 : GenericAttributeSystem2<HealthAttributeComponent> { }
    public class ManaAttributeSystem2 : GenericAttributeSystem2<ManaAttributeComponent> { }

}