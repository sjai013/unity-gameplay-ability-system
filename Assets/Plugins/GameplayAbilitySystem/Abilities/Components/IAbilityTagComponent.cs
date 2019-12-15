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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitySystem.Common.Components;
using GameplayAbilitySystem.GameplayEffects.Components;
using Unity.Entities;

namespace GameplayAbilitySystem.Abilities.Components {
    public interface IAbilityTagComponent {
        int AbilityIdentifier { get; }
        void CreateCooldownEntities(EntityManager dstManager, Entity actorEntity);
        void CommitAbility(EntityManager dstManager, Entity actorEntity);
        void CreateSourceAttributeModifiers(EntityManager dstManager, Entity actorEntity);
        void CreateTargetAttributeModifiers(EntityManager dstManager, Entity actorEntity);

        void BeginActivateAbility(EntityManager dstManager, Entity grantedAbilityEntity);
        void EndActivateAbility(EntityManager dstManager, Entity grantedAbilityEntity);
        object EmptyPayload {get;}

        IEnumerator DoAbility(object Payload);

    }

    public static class AbilityManager {

        public static IEnumerable<ComponentType> AbilityComponentTypes() {
            // Use reflection to get list of all structs that implement IAbilityTagComponent
            // Check each granted ability to see if it has a component that matches the IAbilityTagComponent
            var componentInterface = typeof(IAbilityTagComponent);
            var types = System.AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => componentInterface.IsAssignableFrom(p) && !p.IsInterface && p.IsValueType)
                        .Select(x => (ComponentType)x)
                        .Where(x => x != null);

            return types;
        }
    }

    public struct AbilityCooldownComponent : IComponentData {
        public TimeRemainingComponent Value;
        public static implicit operator TimeRemainingComponent(AbilityCooldownComponent e) { return e.Value; }
        public static implicit operator AbilityCooldownComponent(TimeRemainingComponent e) { return new AbilityCooldownComponent { Value = e }; }
    }

    public struct AbilityStateComponent : IComponentData {
        public int Value;
        public static implicit operator int(AbilityStateComponent e) { return e.Value; }
        public static implicit operator AbilityStateComponent(int e) { return new AbilityStateComponent { Value = e }; }

    }

    public struct AbilityIdentifierComponent : IComponentData {
        public int Value;
        public static implicit operator int(AbilityIdentifierComponent e) { return e.Value; }
        public static implicit operator AbilityIdentifierComponent(int e) { return new AbilityIdentifierComponent { Value = e }; }
    }

}



