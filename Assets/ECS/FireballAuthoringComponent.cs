using System.Collections.Generic;
using GameplayAbilitySystem.Abilities.AbilityActivations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class FireballAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField] GameObject Prefab;
    [SerializeField] AbstractAbilityActivation AbilityActivationBehaviour;
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;



    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var system = World.Active.GetExistingSystem<FireAbilityActivationSystem>();
        system.Prefab = Prefab;
        system.Behaviour = AbilityActivationBehaviour;
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        referencedPrefabs.Add(Prefab);
    }
}

public struct FireballEntitySpawnerComponent : IComponentData {
    public Entity Prefab;
}