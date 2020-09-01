using GameplayAbilitySystem.AttributeSystem.Components;
using GameplayAbilitySystem.AttributeSystem.Systems;
using Unity.Collections;
using Unity.Entities;

namespace MyGameplayAbilitySystem
{
    public class MyAttributeUpdateSystem : AttributeUpdateSystem
    {

        private void Test()
        {
            this.nAttributes = 5;

            var em = this.EntityManager;
            var archetype = em.CreateArchetype(
                typeof(AttributeBufferElement),
                typeof(AttributeModifierBufferElement)
            );

            var entityCount = 5000;

            var entities = new NativeArray<Entity>(entityCount, Allocator.Temp);
            em.CreateEntity(archetype, entities);
            var startWorldTime = Time.ElapsedTime;
            Unity.Mathematics.Random random = new Unity.Mathematics.Random(1);
            for (int i = 0; i < entities.Length; i++)
            {
                var attributeBuffer = em.GetBuffer<AttributeBufferElement>(entities[i]);
                var attributeModifierBuffer = em.GetBuffer<AttributeModifierBufferElement>(entities[i]);

                attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 100, CurrentValue = 100 } });
                attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 100, CurrentValue = 100 } });
                attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 20, CurrentValue = 20 } });
                attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 20, CurrentValue = 20 } });
                attributeBuffer.Add(new AttributeBufferElement() { Value = new AttributeData { BaseValue = 5, CurrentValue = 5 } });

                for (var j = 0; j < nOperators; j++)
                {
                    for (var k = 0; k < this.nAttributes; k++)
                    {
                        var randFloat1 = random.NextFloat(0, 1);
                        attributeModifierBuffer.Add(new AttributeModifierBufferElement() { AttributeId = k, ModifierValue = randFloat1, OperatorId = j });
                    }
                }
            }

            ActorAttributeChanged[entities[0]].OnEvent += (o, e) =>
            {
                //Debug.Log(e.NewAttribute.Length);
            };

            entities.Dispose();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Test();
        }

        protected override void RaiseEvents(int _nAttributes, int nEntities, NativeArray<Entity> modifiedAttributesEntities_ByEntity, NativeArray<AttributeBufferElement> modifiedAttributesOld_ByEntity, NativeArray<AttributeBufferElement> modifiedAttributesNew_ByEntity)
        {

        }


    }
}