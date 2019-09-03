using System;
using Unity.Entities;

public struct CasterAbilityTuple : IComponentData, IEquatable<CasterAbilityTuple> {
    public Entity Host;
    public EAbility Ability;

    public bool Equals(CasterAbilityTuple other) {
        return other.Host == Host && other.Ability == Ability;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + Host.GetHashCode();
            hash = hash * 31 + (int)Ability.GetHashCode();
            return hash;
        }
    }
}
