using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace GameplayAbilitySystem.SCD
{
    public class HealthAttributeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            List<AttributeMasterEntity> entityMasters = new List<AttributeMasterEntity>();
            EntityManager.GetAllUniqueSharedComponentData<AttributeMasterEntity>(entityMasters);
            foreach (AttributeMasterEntity entity in entityMasters)
            {
                NativeArray<float> modifierValues = new NativeArray<float>(3, Allocator.TempJob);
                Entities.WithSharedComponentFilter(entity)
                    .ForEach((ref HealthAttributeModifier health) =>
                    {
                        switch (health.Modifier)
                        {
                            case EAttributeModifiers.Add:
                                modifierValues[0] += health.Value;
                                break;
                        }
                    })
                    .WithBurst()
                    .ScheduleParallel();

                Entities.ForEach((ref HealthCurrentValue currentValue, ref HealthBaseValue baseValue) =>
                {
                    currentValue.Value = modifierValues[0];
                })
                .WithDisposeOnCompletion(modifierValues)
                    .WithBurst()
                .ScheduleParallel();

                // DisplayColor newColor = ColorTable.GetNextColor(cohort.Value);
                // Entities.WithSharedComponentFilter(entity)
                //     .ForEach((ref DisplayColor color) => { color = newColor; })
                //     .ScheduleParallel();
            }
        }
    }
}