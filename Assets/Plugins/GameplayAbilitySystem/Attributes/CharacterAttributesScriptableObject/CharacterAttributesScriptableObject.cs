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

using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Components {
    [CreateAssetMenu(fileName = "ActorAttributes", menuName = "Gameplay Ability System/Attributes/Attributes Prototype")]
    public class CharacterAttributesScriptableObject : ScriptableObject {
        [SerializeField]
        [HideInInspector]
        public List<string> Attributes = new List<string>();

        public List<ComponentType> ComponentArchetype => ConvertAttributesToTypes();
        private List<ComponentType> ConvertAttributesToTypes() {
            List<ComponentType> types = new List<ComponentType>(Attributes.Count);
            for (var i = 0; i < Attributes.Count; i++) {
                types.Add(Type.GetType(Attributes[i]));
            }
            return types;
        }
    }
}