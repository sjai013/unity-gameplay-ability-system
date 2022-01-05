using Unity.Collections;
using Unity.Jobs;
using Unity.Entities;

public class PrimaryAttributeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(7, Allocator.Temp);
        
        
        jobHandles[0] = Entities.ForEach(
            (ref AttributeStrength.CurrentValue currentValue, in AttributeStrength.Modifier modifier, in AttributeStrength.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        jobHandles[1] = Entities.ForEach(
            (ref AttributeIntelligence.CurrentValue currentValue, in AttributeIntelligence.Modifier modifier, in AttributeIntelligence.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        jobHandles[2] = Entities.ForEach(
            (ref AttributeAgility.CurrentValue currentValue, in AttributeAgility.Modifier modifier, in AttributeAgility.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        jobHandles[3] = Entities.ForEach(
            (ref AttributeMaxHealth.CurrentValue currentValue, in AttributeMaxHealth.Modifier modifier, in AttributeMaxHealth.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        jobHandles[4] = Entities.ForEach(
            (ref AttributeHealth.CurrentValue currentValue, in AttributeHealth.Modifier modifier, in AttributeHealth.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        jobHandles[5] = Entities.ForEach(
            (ref AttributeMaxMana.CurrentValue currentValue, in AttributeMaxMana.Modifier modifier, in AttributeMaxMana.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        jobHandles[6] = Entities.ForEach(
            (ref AttributeMana.CurrentValue currentValue, in AttributeMana.Modifier modifier, in AttributeMana.BaseValue baseValue) =>
            {
                currentValue.Value = (int) (baseValue.Value + modifier.Add + (baseValue.Value * modifier.Multiply));
            }).Schedule(this.Dependency);


        this.Dependency = JobHandle.CombineDependencies(jobHandles);
        jobHandles.Dispose();
    }
}