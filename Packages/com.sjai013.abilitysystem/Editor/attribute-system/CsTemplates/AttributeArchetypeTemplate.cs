using System.Collections.Generic;

namespace GameplayAbilitySystem
{

    internal class AttributeArchetypeTemplate : CodeTemplate
    {
        protected override string fileName => "AttributeArchetype.template";

        public string Generate(string name, string description)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("NAME", name),
                ("DESCRIPTION", description)
            });
        }
    }
}