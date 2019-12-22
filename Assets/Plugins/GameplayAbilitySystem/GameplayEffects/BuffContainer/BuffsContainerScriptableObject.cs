/*
 * Created on Sat Dec 21 2019
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

using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Common.ScriptableObjects;
using GameplayAbilitySystem.GameplayEffects.Interfaces;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.ScriptableObjects {
    [CreateAssetMenu(fileName = "BuffsContainer", menuName = "Gameplay Ability System/GameplayEffects/Buffs Container")]
    public class BuffsContainerScriptableObject : AbstractComponentTypeSelectionScriptableObject<IBuff> {
        public List<int> CachedBuffIndices = null;
        public List<int> GetIndices() {
            // If we have already computed the indices, use those
            if (CachedBuffIndices != null && CachedBuffIndices.Count > 0) return CachedBuffIndices;

            // Get the buff index for each ComponentType.
            // We need to use reflection to create a new type
            // Then cast it as an IBuff, and read the IBuff property.
            var componentTypes = ComponentTypes;
            var buffIndices = ComponentTypes.Select(x => ((IBuff)(Activator.CreateInstance(x.GetManagedType()))).BuffIndex).ToList();
            CachedBuffIndices = buffIndices;
            return buffIndices;
        }

    }
}