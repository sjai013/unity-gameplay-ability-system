using System;
using Unity.Entities;

public struct _AbilityComponent : IComponentData, IEquatable<_AbilityComponent> {
    public EAbility Ability;

    public bool Equals(_AbilityComponent other) {
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
