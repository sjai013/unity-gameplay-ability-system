using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public abstract class AbilityCooldownSystem : JobComponentSystem {

    [BurstCompile]
    struct AbilityCooldownSystemJob : IJob {
        // Add fields here that your job needs to do its work.
        // For example,
        //    public float deltaTime;
        public void Execute() {

        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new AbilityCooldownSystemJob();
 
        // Assign values to the fields on your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     job.deltaTime = UnityEngine.Time.deltaTime;



        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(inputDeps);
    }
}