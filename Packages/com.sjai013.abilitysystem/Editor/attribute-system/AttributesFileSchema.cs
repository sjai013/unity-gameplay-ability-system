using System.Collections.Generic;

namespace GameplayAbilitySystem.AttributeSystem.Editor
{
    internal class Modifier
    {
        public string Name { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }

    internal class Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Groups { get; set; }
        public Modifier[] Modifiers { get; set; }
    }

    internal class AttributesFileSchema
    {
        public string Name { get; set; }
        public Attribute[] Attributes { get; set; }
    }
}