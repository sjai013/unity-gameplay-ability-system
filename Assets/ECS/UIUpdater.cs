using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class UIUpdater : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach((Entity entity, HPUIComponent component, ref AttributesComponent attributes) => {
            component.UIAttributeUpdater.UpdateAttributeValue(attributes.Health.CurrentValue, attributes.MaxHealth.CurrentValue);
        });

        Entities.ForEach((Entity entity, ManaUIComponent component, ref AttributesComponent attributes) => {
            component.UIAttributeUpdater.UpdateAttributeValue(attributes.Mana.CurrentValue, attributes.MaxMana.CurrentValue);
        });
    }
}