using Unity.Entities;


public class FireAbilitySystem : AbilitySystem<FireAbility, FireAbilityCost> { }
public struct FireAbility : IAbility, IComponentData {
    public Entity Target { get; set; }
    public Entity Source { get; set; }
}

public struct FireAbilityCost : ICost, IComponentData {
    const int ManaCost = 2;
    public void ApplyGameplayEffect(int index, EntityCommandBuffer.Concurrent Ecb, Entity Source, Entity Target, AttributesComponent attributesComponent) {
        var attributeModData = new AttributeModificationComponent()
        {
            Add = -ManaCost,
            Multiply = 0,
            Divide = 0,
            Change = 0,
            Source = Source,
            Target = Target
        };

        var attributeModEntity = Ecb.CreateEntity(index);
        Ecb.AddComponent(index, attributeModEntity, new ManaAttributeModifier());
        Ecb.AddComponent(index, attributeModEntity, new PermanentAttributeModification());
        Ecb.AddComponent(index, attributeModEntity, attributeModData);
    }

    public bool CheckResourcesAvailable(Entity Caster, AttributesComponent attributes) {
        return attributes.Mana.CurrentValue >= ManaCost;
    }
}