using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class HealAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {
    [SerializeField] GameObject Prefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var system = World.Active.GetExistingSystem<HealAbilityActivationSystem>();
        system.Prefab = Prefab;
    }

}