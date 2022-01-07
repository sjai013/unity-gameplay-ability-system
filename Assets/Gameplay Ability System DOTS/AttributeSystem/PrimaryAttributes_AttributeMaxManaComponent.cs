namespace MyGameplayAbilitySystem.Attributes
{
    /// <summary>
    /// The character's maximum mana
    /// </summary>
    public sealed class AttributeMaxMana : GameplayAbilitySystem.AttributeSystem.IAttribute
    {
        public static Unity.Entities.EntityArchetype GetArchetype(Unity.Entities.EntityManager em) 
        {
            return em.CreateArchetype(GetTypes());
        }

        public static Unity.Entities.ComponentType[] GetTypes() 
        {
            return new Unity.Entities.ComponentType[] {typeof(BaseValue), typeof(CurrentValue), typeof(Modifier)};
        }

        /// <summary>
        /// Base Value of Attribute
        /// </summary>
        public struct BaseValue : Unity.Entities.IComponentData
        {

            /// <summary>
            /// Value of component
            /// </summary>
            public int Value;
        }

        /// <summary>
        /// Current Value of Attribute
        /// </summary>
        public struct CurrentValue : Unity.Entities.IComponentData
        {
            /// <summary>
            /// Value of component
            /// </summary>
            public int Value;
        }

        /// <summary>
        /// Attribute modifiers
        /// </summary>
        public struct Modifier : Unity.Entities.IComponentData
        {
            /// <summary>
            /// Total additive modifiers
            /// </summary>
            public Unity.Mathematics.half Add;

            /// <summary>
            /// Total multiplicative modifiers
            /// </summary>
            public Unity.Mathematics.half Multiply;
        }
    }
}