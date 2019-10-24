using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.JobTypes;
using GameplayAbilitySystem.Attributes.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
namespace MyGameplayAbilitySystem.Attributes.Systems {
    public class HealthAttributeSystem : GenericAttributeSystem<HealthAttributeComponentTag> { }
    public class ManaAttributeSystem : GenericAttributeSystem<ManaAttributeComponentTag> { }

}