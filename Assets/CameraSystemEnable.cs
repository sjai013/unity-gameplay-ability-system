using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class CameraSystemEnable : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent(entity, typeof(CameraComponent));
        dstManager.SetComponentData<CameraComponent>(entity, new CameraComponent {
            isEnabled = true
        });
    } 


}


public struct CameraComponent : IComponentData {
    public bool isEnabled;
}