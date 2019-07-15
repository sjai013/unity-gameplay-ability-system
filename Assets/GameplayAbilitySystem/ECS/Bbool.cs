public struct Bbool {
    public byte Value;

    public Bbool(bool value) {
        Value = (value) ? (byte)1 : (byte)0;
    }

    public static implicit operator Bbool(bool value) {
        return new Bbool(value);
    }

    public static implicit operator bool(Bbool value) {
        return value.Value == 1;
    }
}
