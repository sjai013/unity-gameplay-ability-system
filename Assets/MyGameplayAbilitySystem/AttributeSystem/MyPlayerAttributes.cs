namespace MyGameplayAbilitySystem
{
    public struct MyPlayerAttributes<T>
    where T : struct
    {
        public T Health;
        public T MaxHealth;
        public T Mana;
        public T MaxMana;
        public T Speed;
    }
}
