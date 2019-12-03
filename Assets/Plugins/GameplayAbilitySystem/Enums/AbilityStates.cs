namespace GameplayAbilitySystem.AbilitySystem.Enums {
    [System.Flags]
    public enum AbilityStates {
        READY = 0b_0000_0000_0000_0000,
        ACTIVATING = 0b_0000_0000_0000_0010,
        ON_COOLDOWN = 0b_0000_0000_0000_0100,
        SOURCE_NOT_READY = 0b_0000_0000_0000_1000,

    };
}
