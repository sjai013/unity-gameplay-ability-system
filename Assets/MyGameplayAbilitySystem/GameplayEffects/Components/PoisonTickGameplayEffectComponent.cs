/*
 * Created on Fri Dec 13 2019
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
using GameplayAbilitySystem.Attributes.Components.Operators;
using GameplayAbilitySystem.GameplayEffects.Components;
using GameplayAbilitySystem.GameplayEffects.Interfaces;
using Unity.Entities;


namespace MyGameplayAbilitySystem.GameplayEffects.Components {
    public struct PoisonTickGameplayEffectComponent : IGameplayEffectTagComponent, IComponentData {
        public float Damage;
        private const float DURATION = -1f;
        public Entity Instantiate(EntityManager dstManager, Entity actorEntity, float duration) {
            var archetype = dstManager.CreateArchetype(
                                    typeof(GameplayEffectDurationComponent),
                                    typeof(GameplayEffectTargetComponent),
                                    this.GetType());

            var effectEntity = dstManager.CreateEntity(archetype);
            dstManager.SetComponentData<GameplayEffectTargetComponent>(effectEntity, actorEntity);
            // We use a negative duration to signif y that this gameplay effect should expire straight away
            dstManager.SetComponentData<GameplayEffectDurationComponent>(effectEntity, GameplayEffectDurationComponent.Initialise(DURATION, UnityEngine.Time.time));
            new PermanentAttributeModifierTag() { }.CreateAttributeModifier<HealthAttributeComponent, Add>(dstManager, actorEntity, Damage);
            return effectEntity;
        }

        public Entity Instantiate(int jobIndex, EntityCommandBuffer.Concurrent Ecb, Entity actorEntity, float duration) {
            var entity = Ecb.CreateEntity(jobIndex);
            Ecb.AddComponent<GameplayEffectDurationComponent>(jobIndex, entity, GameplayEffectDurationComponent.Initialise(DURATION, 0f));
            Ecb.AddComponent<GameplayEffectTargetComponent>(jobIndex, entity, actorEntity);
            Ecb.AddComponent(jobIndex, entity, this.GetType());
            new PermanentAttributeModifierTag() { }.CreateAttributeModifier<HealthAttributeComponent, Add>(jobIndex, Ecb, actorEntity, Damage);
            return entity;
        }
    }
}
