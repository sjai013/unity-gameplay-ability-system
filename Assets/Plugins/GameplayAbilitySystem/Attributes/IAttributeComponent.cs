/*
 * Created on Mon Nov 04 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
 
 using Unity.Entities;

namespace GameplayAbilitySystem.Attributes.Components {

    /// <summary>
    /// Attribute types should implement this interface.
    /// <para>
    /// See <see cref="GameplayAbilitySystem.Attributes.Components.AttributeModifier{TOper, TAttribute}"/>
    /// for details on specifying how the attribute will affect attribute values.
    /// </para>
    /// <para>
    /// The <see cref="BaseValue"/> is the "permanent" value for the attribute, and the <see cref="BaseValue"/>
    /// represents temporary/transient modifications to the attribute.  The <see cref="CurrentValue"/>
    /// should be used for calculations.  The <see cref="BaseValue"/> is only used as a means of 
    /// calculating the <see cref="CurrentValue"/>.
    /// </para>
    /// <para>
    /// For example, a particular item might grant the player a 10% damage boost.
    /// If the player's damage <see cref="BaseValue"/> is 100, then the <see cref="CurrentValue"/> would be 110.
    /// </para>
    /// </summary>
    public interface IAttributeComponent {
        float BaseValue { get; set; }
        float CurrentValue { get; set; }
    }

    /// <summary>
    /// Operators are used to control how different modifiers interact.
    /// New classes can implement this interface, and then specify the implementation
    /// in a custom AttributeSystem
    /// </summary>
    public interface IAttributeOperator { }

    /// <summary>
    /// Attribute modifiers are tagged with the <see cref="AttributeComponentTag{TAttributeComponent}"/> component.
    /// The presence of this tag is essential for the attribute modifier to be counted.
    /// </summary>
    public struct AttributeComponentTag<TAttributeComponent> : IComponentData
    where TAttributeComponent : struct, IAttributeComponent, IComponentData { }

}

namespace GameplayAbilitySystem.Attributes.Components.Operators {

    /// <summary>
    /// The Add operator is used to indicate that these modifiers should be added to the attribute
    /// </summary>
    public struct Add : IAttributeOperator, IComponentData { }
    /// <summary>
    /// The Multiply operator is used to indicate that these modifiers should be multiplied to the base attribute
    /// </summary>
    public struct Multiply : IAttributeOperator, IComponentData { }
    /// <summary>
    /// The Divide operator is used to indicate that these modifiers should be divided from the base attribute. 
    /// </summary>
    public struct Divide : IAttributeOperator, IComponentData { }
}