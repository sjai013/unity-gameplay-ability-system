using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class AttributeSystem : JobComponentSystem {

    private struct AttributeJob : IJobForEachWithEntity<AttributesComponent> {

        public void Execute(Entity entity, int index, ref AttributesComponent data) {
            

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new AttributeJob();
        return job.Schedule(this, inputDeps);
    }
}