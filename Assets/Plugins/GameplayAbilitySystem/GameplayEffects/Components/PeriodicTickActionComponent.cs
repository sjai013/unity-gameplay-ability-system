/*
 * Created on Thu Dec 12 2019
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

using System.Runtime.InteropServices;
using GameplayAbilitySystem.Common.Components;
using Unity.Burst;
using Unity.Entities;

namespace GameplayAbilitySystem.GameplayEffects.Components {
    public struct PeriodicTickActionComponent<T> : IComponentData
    where T : System.Delegate {
        public FunctionPointer<T> Tick;
        public static EntityArchetype CreateArchetype(EntityManager entityManager) {
            return entityManager.CreateArchetype(
                typeof(PeriodicTickComponent),
                typeof(PeriodicTickActionComponent<T>),
                typeof(ParentGameplayEffectEntity),
                typeof(PeriodicTickTargetComponent)
            );
        }

        public static PeriodicTickActionComponent<T> Instantiate(T d) {
            return new PeriodicTickActionComponent<T>()
            {
                Tick = new FunctionPointer<T>(
                            Marshal.GetFunctionPointerForDelegate(
                                d))
            };
        }
    }
}