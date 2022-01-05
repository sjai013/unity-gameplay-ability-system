using System.Collections.Generic;

namespace GameplayAbilitySystem
{

    internal class CVSystemTemplate : CodeTemplate
    {
        protected override string fileName => "CVSystem.template";

        public string Generate(int length, string name, string jobs)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("NAME", name),
                ("LENGTH", length.ToString()),
                ("JOBS", jobs)
            });
        }
    }
}