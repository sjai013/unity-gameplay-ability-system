using MyGameplayAbilitySystem;
using Unity.Entities;
using UnityEngine;

public class MyPlayerAttributeAuthoringScript : MonoBehaviour, IConvertGameObjectToEntity
{
    public EntityManager dstManager { get; private set; }
    public Entity attributeEntity { get; private set; }

    public int maxHitPoints;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var defaultAttributes = new MyPlayerAttributes<uint>() { Health = (uint)maxHitPoints, MaxHealth = (uint)maxHitPoints };
        attributeEntity = MyAttributeUpdateSystem.CreatePlayerEntity(dstManager, new AttributeValues() { BaseValue = defaultAttributes });
        dstManager.SetName(attributeEntity, $"{this.gameObject.name} - Attributes");
        this.dstManager = dstManager;
    }
}
