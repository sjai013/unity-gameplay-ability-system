using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameplayAbilitySystem.Abilities.Components;
using GameplayAbilitySystem.AbilitySystem.Components;
using MyGameplayAbilitySystem.Abilities;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem.InputSystem;

[RequireComponent(typeof(ActorAbilitySystem))]
public class ActorCast : MonoBehaviour, ICastActions {

    private InputSystem.InputSystem controls;
    private ActorAbilitySystem actorAbilitySystem;

    private EntityQuery grantedAbilityQuery;

    public List<int> CastAbilityButtonMap = new List<int>();

    List<Entity> GrantedAbilityEntities;
    List<(IAbilityTagComponent AbilityTag, ComponentType ComponentType, Entity GrantedAbilityEntity)> GrantedAbilities;

    public void OnCast1(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(1);
    }

    public void OnCast2(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(2);
    }

    public void OnCast3(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(3);
    }

    public void OnCast4(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(4);
    }

    public void OnCast5(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(5);
    }

    public void OnCast6(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(6);
    }

    public void OnCast7(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(7);
    }

    public void OnCast8(InputAction.CallbackContext context) {
        if (!context.performed) return;
        Cast(8);
    }


    // Start is called before the first frame update
    void Start() {
        actorAbilitySystem = GetComponent<ActorAbilitySystem>();
        if (controls == null) {
            controls = new InputSystem.InputSystem();
            controls.Cast.SetCallbacks(this);
        }
        controls.Cast.Enable();

        GetAllAbilities();
        // Get list of grantedAbility entities
        GetGrantedAbilities(World.Active.EntityManager);
    }

    void GetAllAbilities() {
        var abilityComponentTypes = AbilityManager.AbilityComponentTypes();

    }

    /// <summary>
    /// Get list of granted abilities and store them
    /// </summary>
    /// <param name="entityManager">Entity Manager</param>
    void GetGrantedAbilities(EntityManager entityManager) {
        MethodInfo methodInfo = typeof(EntityManager).GetMethod("GetComponentData");
        if (GrantedAbilities == null) GrantedAbilities = new List<(IAbilityTagComponent AbilityTag, ComponentType ComponentType, Entity GrantedAbilityEntity)>();

        grantedAbilityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<AbilityOwnerComponent>(), ComponentType.ReadOnly<AbilityStateComponent>());
        var grantedAbilityEntities = grantedAbilityQuery.ToEntityArray(Allocator.TempJob, out var jobHandle);
        jobHandle.Complete();
        var abilities = AbilityManager.AbilityComponentTypes().ToList();
        for (var i = 0; i < grantedAbilityEntities.Length; i++) {
            var abilityOwner = entityManager.GetComponentData<AbilityOwnerComponent>(grantedAbilityEntities[i]);
            // We need to do this only for abilities that have been granted to the actor that has this script.  Others should be ignored
            if (abilityOwner != actorAbilitySystem.AbilityOwnerEntity) continue;

            var abilityState = entityManager.GetComponentData<AbilityStateComponent>(grantedAbilityEntities[i]);
            var grantedAbilityEntity = grantedAbilityEntities[i];

            for (var iComponentType = 0; iComponentType < abilities.Count; iComponentType++) {

                if (entityManager.HasComponent(grantedAbilityEntity, abilities[iComponentType])) {
                    // Create instance of the ability to store locally, cast to the IAbilityTagComponent interface, so we can call things on it
                    IAbilityTagComponent abilityTagComponent = (IAbilityTagComponent)Activator.CreateInstance(abilities[iComponentType].GetManagedType()); ;
                    GrantedAbilities.Add((abilityTagComponent, abilities[iComponentType], grantedAbilityEntity));
                }
            }
        }
        grantedAbilityEntities.Dispose();
    }



    void Cast(int abilityId) {
        // Find this ability in the GrantedAbilities array
        var ability = GrantedAbilities.Find(x => x.AbilityTag.AbilityIdentifier == abilityId);
        if (ability.AbilityTag == null) return;

        var payload = new BasicAbilityPayload();
        payload.ActorAbilitySystem = this.actorAbilitySystem;
        payload.ActorTransform = this.transform;
        payload.EntityManager = World.Active.EntityManager;
        payload.GrantedAbilityEntity = ability.GrantedAbilityEntity;
        StartCoroutine(ability.AbilityTag.DoAbility(payload));
    }


    // private IEnumerator DoDefaultAttack(EntityManager entityManager, Entity entity, ActorAbilitySystem actorAbilitySystem) {
    //     var translation = entityManager.GetComponentData<Translation>(entity);
    //     var rotation = entityManager.GetComponentData<Rotation>(entity);
    //     var AbilityState = -1;
    //     Entity GrantedAbilityEntity = default(Entity);
    //     var forwardVector = math.normalize(math.mul(rotation.Value, new float3(0, 0, 1)));

    //     // Get all entities that have these components
    //     // Check if player can use ability (the GrantedAbility entity contains this information)
    //     Entities
    //         .WithAllReadOnly<AbilityOwnerComponent, AbilityStateComponent, DefaultAttackAbilityTag>().ForEach((Entity grantedAbilityEntity, ref AbilityStateComponent abilityState, ref AbilityOwnerComponent abilityOwner) => {
    //             if (abilityOwner == actorAbilitySystem.AbilityOwnerEntity) {
    //                 AbilityState = abilityState;
    //                 GrantedAbilityEntity = grantedAbilityEntity;
    //             }
    //         });

    //     if (AbilityState != 0) {
    //         // We could also do checks here to see what the ability state is and report an appropriate error on screen
    //         // For example, we are using AbilityState = 2 to note that the ability is on cooldown, so we could display
    //         // something like "This ability is on cooldown" on the screen.
    //         yield break;
    //     }




    //     var playerCastHelper = actorAbilitySystem.GetComponent<PlayerCastTimelineHelperMono>();

    //     // Raycast forward, and if we hit something, reduce it's HP.
    //     RaycastHit hit;
    //     float3 rayOrigin = actorAbilitySystem.CastPoint.transform.position;
    //     // Does the ray intersect any objects
    //     if (!Physics.Raycast(rayOrigin, forwardVector, out hit, Mathf.Infinity)) {
    //         yield break;
    //     }
    //     bool wasHit = false;
    //     Entity targetEntity = default(Entity);
    //     if (hit.distance < 1f) {
    //         Debug.DrawRay(rayOrigin, forwardVector * hit.distance, Color.black, 1f);
    //         if (hit.transform.parent.gameObject.TryGetComponent<ActorAbilitySystem>(out var targetActorAbilitySystemMono)) {
    //             targetEntity = targetActorAbilitySystemMono.AbilityOwnerEntity;
    //             wasHit = true;
    //         }
    //     }

    //     new DefaultAttackAbilityTag().BeginActivateAbility(EntityManager, GrantedAbilityEntity);

    //     playerCastHelper.PlaySwingAnimation();
    //     yield return playerCastHelper.StartCoroutine(playerCastHelper.CheckForSwingHit(wasHit, EntityManager, targetEntity));
    //     // Default attack - trigger cooldown and resource cost
    //     (new DefaultAttackAbilityTag()).CommitAbility(EntityManager, actorAbilitySystem.AbilityOwnerEntity);
    //     new DefaultAttackAbilityTag().EndActivateAbility(EntityManager, GrantedAbilityEntity);

    // }
}
