using Unity.Entities;

namespace GameplayAbilitySystem.AbilitySystem.Components {

    public struct AbilityOwnerComponent : IComponentData {
        public Entity Value;
        public static implicit operator Entity(AbilityOwnerComponent e) { return e.Value; }
        public static implicit operator AbilityOwnerComponent(Entity e) { return new AbilityOwnerComponent { Value = e }; }
    }

}