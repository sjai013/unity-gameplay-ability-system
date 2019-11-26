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
using GameplayAbilitySystem.Attributes.Components;
using Operators = GameplayAbilitySystem.Attributes.Components.Operators;
using GameplayAbilitySystem.Common.Editor;

[assembly: RegisterGenericComponentType(typeof(AttributeComponentTag<ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Add, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Multiply, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModifier<Operators.Divide, ManaAttributeComponent>))]

namespace GameplayAbilitySystem.Attributes.Components {
    [AbilitySystemDisplayName("Mana")]
    public struct ManaAttributeComponent : IComponentData, IAttributeComponent {
        public float _baseValue;
        public float _currentValue;
        public float BaseValue { get => _baseValue; set => _baseValue = value; }
        public float CurrentValue { get => _currentValue; set => _currentValue = value; }
    }
}