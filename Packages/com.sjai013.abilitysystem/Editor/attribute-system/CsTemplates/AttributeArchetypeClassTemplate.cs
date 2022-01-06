using System.Collections.Generic;

namespace GameplayAbilitySystem
{

    internal class AttributeArchetypeClassTemplate : CodeTemplate
    {
        protected override string fileName => "AttributeArchetypeClass.template";

        public string Generate(string name, string content)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("NAME", name),
                ("CONTENT", content)
            });
        }
    }
}