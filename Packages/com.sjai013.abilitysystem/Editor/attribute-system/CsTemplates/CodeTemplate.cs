using System.Collections.Generic;
using System.IO;

namespace GameplayAbilitySystem
{
    internal abstract class CodeTemplate
    {
        protected const string basePath = "Packages/com.sjai013.abilitysystem/Editor/attribute-system/CsTemplates";
        protected string filePath { get => basePath + "/" + fileName; }
        protected abstract string fileName { get; }

        internal string Replace(List<(string From, string To)> replaceWith)
        {
            var code = File.ReadAllText(filePath);
            for (var i = 0; i < replaceWith.Count; i++)
            {
                code = code.Replace("{" + replaceWith[i].From + "}", replaceWith[i].To);
            }
            return code;
        }
    }
}