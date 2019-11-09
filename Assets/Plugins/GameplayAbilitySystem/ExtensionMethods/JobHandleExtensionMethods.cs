using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using static Unity.Entities.JobForEachExtensions;

namespace GameplayAbilitySystem.ExtensionMethods {
    public static class JobHandleExtensionMethods {
        public static JobHandle ScheduleJob<T>(this JobHandle jobHandle, T jobData, EntityQuery query)
        where T : struct, IBaseJobForEach {
            return jobData.Schedule(query, jobHandle);
        }

    }
}
