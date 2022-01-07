using System.Collections.Generic;

namespace GameplayAbilitySystem
{

    internal class CVSystemTemplate : CodeTemplate
    {
        protected override string fileName => "CVSystem.template";

        public string Generate(int length, string @namespace, string name, string jobs, string version, string sourcePath)
        {
            return Replace(new List<(string From, string To)>()
            {
                ("NAMESPACE", @namespace),
                ("NAMESPACE", @namespace),
                ("NAME", name),
                ("LENGTH", length.ToString()),
                ("JOBS", jobs),
                ("VERSION", version),
                ("SOURCE_PATH", sourcePath)
            });
        }
    }
}