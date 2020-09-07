using Gamekit3D;
using MyGameplayAbilitySystem;
using Unity.Entities;
using UnityEngine;

public class MyPlayerAttributeAuthoringScript : MonoBehaviour, IConvertGameObjectToEntity
{
    public EntityManager dstManager { get; private set; }
    public Entity attributeEntity { get; private set; }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        attributeEntity = InitialiseAttributeEntity(dstManager);
        this.dstManager = dstManager;
    }

    public Entity InitialiseAttributeEntity(EntityManager dstManager)
    {
        var damagable = GetComponent<Damageable>();
        var defaultAttributes = new MyPlayerAttributes<uint>() { Health = (uint)damagable.maxHitPoints, MaxHealth = (uint)damagable.maxHitPoints };
        attributeEntity = MyAttributeUpdateSystem.CreatePlayerEntity(dstManager, new AttributeValues() { BaseValue = defaultAttributes });
        dstManager.SetName(attributeEntity, $"{this.gameObject.name} - Attributes");
        return attributeEntity;
    }
}
