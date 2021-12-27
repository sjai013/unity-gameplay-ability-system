using System.Collections.Generic;

namespace GameplayAbilitySystem.AttributeSystem.Editor
{
    public class Clamp
    {
        public string Attribute { get; set; }
    }

    public class AttributeModifier
    {
        public string Modifier { get; set; }
        public List<string> Parameters { get; set; }
    }

    public class Attribute
    {
        public string AttributeName { get; set; }
        public string AttributeDescription { get; set; }
        public List<AttributeModifier> AttributeModifiers { get; set; }
    }

    public class AttributeGroupFileSchema
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public List<Attribute> Attributes { get; set; }
    }

}