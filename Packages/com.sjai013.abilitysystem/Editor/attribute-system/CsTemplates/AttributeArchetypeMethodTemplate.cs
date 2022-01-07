using System;
using System.Collections.Generic;
using System.Linq;

namespace GameplayAbilitySystem
{

    internal class AttributeArchetypeMethodTemplate : CodeTemplate
    {
        protected override string fileName => "AttributeArchetypeMethod.template";

        public string Generate(string name, List<string> types)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("NAME", name),
                ("CONTENT", string.Join(",",types.Select(x => $"{x}.GetTypes()")))
            });
        }
    }
}