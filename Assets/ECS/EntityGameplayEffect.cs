using System;
using Unity.Entities;

public struct EntityGameplayEffect : IComponentData, IEquatable<EntityGameplayEffect> {
    public Entity Caster;
    public EGameplayEffect Effect;

    public bool Equals(EntityGameplayEffect other) {
        return other.Caster == Caster && other.Effect == Effect;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 31 + Caster.Index.GetHashCode();
            hash = hash * 31 + (int)Effect.GetHashCode();
            return hash;
        }
    }
}