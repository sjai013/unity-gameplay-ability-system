
        /// <summary>
        /// The character's strength
        /// </summary>
        public sealed class AttributeStrength : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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


        /// <summary>
        /// The character's intelligence
        /// </summary>
        public sealed class AttributeIntelligence : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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


        /// <summary>
        /// The character's agility
        /// </summary>
        public sealed class AttributeAgility : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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


        /// <summary>
        /// The character's maximum health
        /// </summary>
        public sealed class AttributeMaxHealth : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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


        /// <summary>
        /// The character health
        /// </summary>
        public sealed class AttributeHealth : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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


        /// <summary>
        /// The character's maximum mana
        /// </summary>
        public sealed class AttributeMaxMana : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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


        /// <summary>
        /// The character mana
        /// </summary>
        public sealed class AttributeMana : GameplayAbilitySystem.AttributeSystem.IAttribute
        {

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
