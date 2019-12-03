/*
 * Created on Tue Dec 03 2019
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

using System.Collections;
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.AbilitySystem.Components;
using MyGameplayAbilitySystem.Abilities;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCastActionSystem : ComponentSystem {
    protected override void OnUpdate() {

        Entities.WithAll<PlayerCastingControllableTagComponent>().ForEach((Entity entity, ActorAbilitySystem actorAbilitySystem) => {
            if (Input.GetKeyUp("1")) {
                actorAbilitySystem.StartCoroutine(DoDefaultAttack(entity, actorAbilitySystem));
            }

        });
    }

    private IEnumerator DoDefaultAttack(Entity entity, ActorAbilitySystem actorAbilitySystem) {
        var translation = GetComponentDataFromEntity<Translation>()[entity];
        var rotation = GetComponentDataFromEntity<Rotation>()[entity];
        var AbilityState = -1;
        Entity GrantedAbilityEntity = default(Entity);
        var forwardVector = math.normalize(math.mul(rotation.Value, new float3(0, 0, 1)));

        // Check if player can use ability (the GrantedAbility entity contains this information)
        Entities
            .WithAllReadOnly<AbilityOwnerComponent, AbilityStateComponent, DefaultAttackAbilityTag>().ForEach((Entity grantedAbilityEntity, ref AbilityStateComponent abilityState, ref AbilityOwnerComponent abilityOwner) => {
                if (abilityOwner == actorAbilitySystem.AbilityOwnerEntity) {
                    AbilityState = abilityState;
                    GrantedAbilityEntity = grantedAbilityEntity;
                }
            });

        if (AbilityState != 0) {
            // We could also do checks here to see what the ability state is and report an appropriate error on screen
            // For example, we are using AbilityState = 2 to note that the ability is on cooldown, so we could display
            // something like "This ability is on cooldown" on the screen.
            yield break;
        }




        var playerCastHelper = actorAbilitySystem.GetComponent<PlayerCastTimelineHelperMono>();

        // Raycast forward, and if we hit something, reduce it's HP.
        RaycastHit hit;
        float3 rayOrigin = actorAbilitySystem.CastPoint.transform.position;
        // Does the ray intersect any objects
        if (!Physics.Raycast(rayOrigin, forwardVector, out hit, Mathf.Infinity)) {
            yield break;
        }
        bool wasHit = false;
        Entity targetEntity = default(Entity);
        if (hit.distance < 1f) {
            Debug.DrawRay(rayOrigin, forwardVector * hit.distance, Color.black, 1f);
            if (hit.transform.parent.gameObject.TryGetComponent<ActorAbilitySystem>(out var targetActorAbilitySystemMono)) {
                targetEntity = targetActorAbilitySystemMono.AbilityOwnerEntity;
                wasHit = true;
            }
        }

        new DefaultAttackAbilityTag().BeginActivateAbility(EntityManager, GrantedAbilityEntity);

        playerCastHelper.PlaySwingAnimation();
        yield return playerCastHelper.StartCoroutine(playerCastHelper.CheckForSwingHit(wasHit, EntityManager, targetEntity));
        // Default attack - trigger cooldown and resource cost
        (new DefaultAttackAbilityTag()).CommitAbility(EntityManager, actorAbilitySystem.AbilityOwnerEntity);
        new DefaultAttackAbilityTag().EndActivateAbility(EntityManager, GrantedAbilityEntity);

    }

}