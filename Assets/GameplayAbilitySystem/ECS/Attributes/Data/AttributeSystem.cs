using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

//public class AttributeSystem : JobComponentSystem {
//    private struct AttributeJob : IJobForEach<AttributesComponent> {
//        public void Execute(ref AttributesComponent data) {
//        }
//    }

//    protected override JobHandle OnUpdate(JobHandle inputDeps) {
//        var job = new AttributeJob();
//        return job.Schedule(this, inputDeps);
//    }
//}