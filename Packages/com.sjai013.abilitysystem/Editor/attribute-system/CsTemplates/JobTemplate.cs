using System.Collections.Generic;

namespace GameplayAbilitySystem
{

    internal class JobTemplate : CodeTemplate
    {
        protected override string fileName => "Job.template";

        public string Generate(int index, string attribute)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("INDEX", index.ToString()),
                ("ATTRIBUTE", attribute)
            });
        }
    }
}