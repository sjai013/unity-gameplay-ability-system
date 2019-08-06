using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class ResetAttributesDeltaSystem : JobComponentSystem {

    [BurstCompile]
    public struct Job : IJobForEach<AttributesComponent> {
        public void Execute(ref AttributesComponent attributesComponent) {
            attributesComponent.MaxHealth.TempDelta = 0;
            attributesComponent.Health.TempDelta = 0;
            attributesComponent.Mana.TempDelta = 0;
            attributesComponent.MaxMana.TempDelta = 0;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        var job = new Job();
        var jobHandle = job.Schedule(this, inputDependencies);
        return jobHandle;
    }
}


public class ApplyAttributesDeltaSystem : JobComponentSystem {

    [BurstCompile]
    public struct Job : IJobForEach<AttributesComponent> {
        public void Execute(ref AttributesComponent attributesComponent) {
            attributesComponent.MaxHealth.CurrentValue = attributesComponent.MaxHealth.BaseValue + attributesComponent.MaxHealth.TempDelta;
            attributesComponent.Health.CurrentValue = attributesComponent.Health.BaseValue + attributesComponent.Health.TempDelta;
            attributesComponent.Mana.CurrentValue = attributesComponent.Mana.BaseValue + attributesComponent.Mana.TempDelta;
            attributesComponent.MaxMana.CurrentValue = attributesComponent.MaxMana.BaseValue + attributesComponent.MaxMana.TempDelta;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        var job = new Job();
        var jobHandle = job.Schedule(this, inputDependencies);
        return jobHandle;
    }
}