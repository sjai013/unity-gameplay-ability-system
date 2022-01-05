namespace GameplayAbilitySystem.AttributeSystem
{
    public interface IAttribute { }
    public interface IAttributeGroup { }

    public struct GenericBaseValue
    {
        /// <summary>
        /// Value of component
        /// </summary>        
        public int Value;
    }

    public struct GenericCurrentValue
    {
        /// <summary>
        /// Total additive modifiers
        /// </summary>
        public short Add;

        /// <summary>
        /// Total multiplicative modifiers
        /// </summary>
        public short Multiply;

        /// <summary>
        /// Value of component
        /// </summary>
        public int Value;
    }

}