using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MousePositionComponent : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent(entity, typeof(MouseScreenPosition));
        dstManager.AddComponent(entity, typeof(MouseDelta));
    }

}
