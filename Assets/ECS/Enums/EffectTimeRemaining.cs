using System;

public struct EffectTimeRemaining : IEquatable<EffectTimeRemaining> {
    public EGameplayEffect Effect;
    public float Remaining;

    public override int GetHashCode() {
        return Effect.GetHashCode();
    }

    public override bool Equals(object obj) {
        return Equals((EffectTimeRemaining)obj);

    }

    public bool Equals(EffectTimeRemaining other) {
        return other.Effect == Effect;
    }

}
