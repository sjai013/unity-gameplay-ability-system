using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[assembly: RegisterGenericComponentType(typeof(GenericComponent<int>))]
[assembly: RegisterGenericComponentType(typeof(GenericComponent<float>))]
public struct GenericComponent<T> : IComponentData {
    public int V;
}
public class EcsTestMono : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent(entity, typeof(GenericComponent<float>));
        dstManager.AddComponent(entity, typeof(GenericComponent<int>));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct SystemStateC : ISystemStateComponentData {

}

