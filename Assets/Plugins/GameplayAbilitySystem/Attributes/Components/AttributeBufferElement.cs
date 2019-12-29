/*
 * Created on Sun Dec 29 2019
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
    public struct AttributeBufferElement<TModifierTag, TAttribute> : IBufferElementData
    where TModifierTag : struct, IAttributeModifierTag, IComponentData
    where TAttribute : struct, IAttributeComponent, IComponentData {
        public Entity Value;
        public static implicit operator Entity(AttributeBufferElement<TModifierTag, TAttribute> e) { return e.Value; }
        public static implicit operator AttributeBufferElement<TModifierTag, TAttribute>(Entity e) { return new AttributeBufferElement<TModifierTag, TAttribute> { Value = e }; }
    }
}