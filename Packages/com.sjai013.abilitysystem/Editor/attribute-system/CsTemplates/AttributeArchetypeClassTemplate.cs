using System.Collections.Generic;

namespace GameplayAbilitySystem
{

    internal class AttributeArchetypeClassTemplate : CodeTemplate
    {
        protected override string fileName => "AttributeArchetypeClass.template";

        public string Generate(string @namespace, string name, string content)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("NAMESPACE", @namespace),
                ("NAME", name),
                ("CONTENT", content)
            });
        }
    }
}