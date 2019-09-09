using System;
using Unity.Entities;

public struct AbilityComponent : IComponentData, IEquatable<AbilityComponent> {
    public EAbility Ability;

    public bool Equals(AbilityComponent other) {
        if (Ability == other.Ability) return true;
        return false;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + (int)Ability.GetHashCode();
            return hash;
        }
    }
}
