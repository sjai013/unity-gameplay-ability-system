using Unity.Entities;

public partial struct AttributesComponent : IAttributeComponentData {
    public BaseAttributeComponent Health;
    public BaseAttributeComponent MaxHealth;
    public BaseAttributeComponent Mana;
    public BaseAttributeComponent MaxMana;
}