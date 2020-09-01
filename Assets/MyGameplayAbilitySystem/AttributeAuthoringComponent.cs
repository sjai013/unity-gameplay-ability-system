using GameplayAbilitySystem.AttributeSystem.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MyGameplayAbilitySystem
{

    [DisallowMultipleComponent]
    public class AttributeAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {

        public Entity AttributeEntity;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
            //
            // * You can add more than one component to the entity. It's also OK to not add any at all.
            //
            // * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
            //   to do it, instead of adding entities through 'dstManager' directly.
            //
            // For example,
            //   dstManager.AddComponentData(entity, new Unity.Transforms.Scale { Value = scale });


            // Create attribute buffer for character
            var archetype = dstManager.CreateArchetype(
                typeof(AttributeBufferElement),
                typeof(AttributeModifierBufferElement)
            );
            var attributeEntity = dstManager.CreateEntity(archetype);
            this.AttributeEntity = attributeEntity;

            var attributeBuffer = dstManager.GetBuffer<AttributeBufferElement>(attributeEntity);
            attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 100, CurrentValue = 100 } });
            attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 100, CurrentValue = 100 } });
            attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 20, CurrentValue = 20 } });
            attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 20, CurrentValue = 20 } });
            attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 5, CurrentValue = 5 } });
        }
    }

}