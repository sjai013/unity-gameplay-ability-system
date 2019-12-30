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

using GameplayAbilitySystem.Attributes.Components;
using GameplayAbilitySystem.Attributes.Systems;
using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<TemporaryAttributeModifierTag, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<TemporaryAttributeModifierTag, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<TemporaryAttributeModifierTag, MaxHealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<TemporaryAttributeModifierTag, MaxManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<TemporaryAttributeModifierTag, CharacterLevelAttributeComponent>))]

[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<PermanentAttributeModifierTag, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<PermanentAttributeModifierTag, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<PermanentAttributeModifierTag, MaxHealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<PermanentAttributeModifierTag, MaxManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeModificationActivatedSystemStateComponent<PermanentAttributeModifierTag, CharacterLevelAttributeComponent>))]

[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<TemporaryAttributeModifierTag, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<TemporaryAttributeModifierTag, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<TemporaryAttributeModifierTag, MaxHealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<TemporaryAttributeModifierTag, MaxManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<TemporaryAttributeModifierTag, CharacterLevelAttributeComponent>))]

[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<PermanentAttributeModifierTag, HealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<PermanentAttributeModifierTag, ManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<PermanentAttributeModifierTag, MaxHealthAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<PermanentAttributeModifierTag, MaxManaAttributeComponent>))]
[assembly: RegisterGenericComponentType(typeof(AttributeBufferElement<PermanentAttributeModifierTag, CharacterLevelAttributeComponent>))]

namespace MyGameplayAbilitySystem.Attributes.Systems {

    public class HealthAttributePermanentSystem : GenericAttributePermanentSystem<HealthAttributeComponent> { }
    public class ManaAttributePermanentSystem : GenericAttributePermanentSystem<ManaAttributeComponent> { }
    public class MaxHealthAttributePermanentSystem : GenericAttributePermanentSystem<MaxHealthAttributeComponent> { }
    public class MaxManaAttributePermanentSystem : GenericAttributePermanentSystem<MaxManaAttributeComponent> { }
    public class CharacterLevelAttributePermanentSystem : GenericAttributePermanentSystem<CharacterLevelAttributeComponent> { }

    public class HealthAttributeTemporarySystem : GenericAttributeTemporarySystem<HealthAttributeComponent> { }
    public class ManaAttributeTemporarySystem : GenericAttributeTemporarySystem<ManaAttributeComponent> { }
    public class MaxHealthAttributeTemporarySystem : GenericAttributeTemporarySystem<MaxHealthAttributeComponent> { }
    public class MaxManaAttributeTemporarySystem : GenericAttributeTemporarySystem<MaxManaAttributeComponent> { }
    public class CharacterLevelAttributeTemporarySystem : GenericAttributeTemporarySystem<CharacterLevelAttributeComponent> { }

    public class TemporaryHealthAttributeInitialiseSystem : AttributeInitialisationSystem<TemporaryAttributeModifierTag, HealthAttributeComponent> { }
    public class TemporaryManaAttributeInitialiseSystem : AttributeInitialisationSystem<TemporaryAttributeModifierTag, ManaAttributeComponent> { }
    public class TemporaryMaxHealthAttributeInitialiseSystem : AttributeInitialisationSystem<TemporaryAttributeModifierTag, MaxHealthAttributeComponent> { }
    public class TemporaryMaxManaAttributeInitialiseSystem : AttributeInitialisationSystem<TemporaryAttributeModifierTag, MaxManaAttributeComponent> { }
    public class TemporaryCharacterLevelAttributeInitialiseSystem : AttributeInitialisationSystem<TemporaryAttributeModifierTag, CharacterLevelAttributeComponent> { }


}