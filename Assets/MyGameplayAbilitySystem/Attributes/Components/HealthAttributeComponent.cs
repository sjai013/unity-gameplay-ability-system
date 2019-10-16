using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameplayAbilitySystem {

    public struct HealthAttributeComponent : IComponentData, IAttributeComponent {
        public int BaseValue;
        public int CurrentValue;

    }
}
