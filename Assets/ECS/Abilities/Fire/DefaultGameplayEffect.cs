using Unity.Entities;

public struct DefaultGameplayEffect {

    public void ApplyGameplayEffect<T0>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect {
        var effect = new T0();
        effect.Source = Source;
        effect.Target = Target;
        effect.ApplyGameplayEffect(index, Ecb, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2, T3>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect
    where T3 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T3>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2, T3, T4>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect
    where T3 : struct, IComponentData, IGameplayEffect
    where T4 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T3>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T4>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

    public void ApplyGameplayEffect<T0, T1, T2, T3, T4, T5>(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent, float WorldTime)
    where T0 : struct, IComponentData, IGameplayEffect
    where T1 : struct, IComponentData, IGameplayEffect
    where T2 : struct, IComponentData, IGameplayEffect
    where T3 : struct, IComponentData, IGameplayEffect
    where T4 : struct, IComponentData, IGameplayEffect
    where T5 : struct, IComponentData, IGameplayEffect {
        ApplyGameplayEffect<T0>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T1>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T2>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T3>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T4>(index, Ecb, Source, Target, attributesComponent, WorldTime);
        ApplyGameplayEffect<T5>(index, Ecb, Source, Target, attributesComponent, WorldTime);
    }

}