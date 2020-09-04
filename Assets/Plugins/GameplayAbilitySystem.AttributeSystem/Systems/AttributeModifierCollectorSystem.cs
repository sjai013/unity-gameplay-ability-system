using Unity.Collections;
using Unity.Entities;

namespace GameplayAbilitySystem.AttributeSystem.Systems
{
    // public class AttributeModifierCollectorSystem<T1, T2> : SystemBase
    // // where T1 : struct, IBufferElementData, IGameplayAttributeModifier<T2>
    // // where T2 : struct, IAttributeModifier, IComponentData
    // {
    //     private EntityQuery m_AttributeModifiers;
    //     protected override void OnCreate()
    //     {
    //         m_AttributeModifiers = GetEntityQuery(typeof(T1));
    //     }
    //     protected override void OnUpdate()
    //     {

    //         // 1. Gather all attribute modifier components
    //         // EITHER:
    //         // 2a. Modify value of TAttributeModifier component to reflect the mods
    //         // OR
    //         // 2b. Keep those values in memory (NativeStream, NativeMultiHashMap, w/e), and apply directly to attributes
    //         NativeHashMap<Entity, T2> a = new NativeHashMap<Entity, T2>(m_AttributeModifiers.CalculateEntityCount(), Allocator.TempJob);
    //         NativeMultiHashMap<Entity, T2> b = new NativeMultiHashMap<Entity, T2>(m_AttributeModifiers.CalculateEntityCount(), Allocator.TempJob);
    //     }

    //     struct AJob : IJobChunk
    //     {
    //         NativeMultiHashMap<Entity, T2> b;

    //         public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    //         {
    //             throw new System.NotImplementedException();
    //         }
    //     }
    // }
}
