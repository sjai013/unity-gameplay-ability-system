using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace GameplayAbilitySystem.AttributeSystem.Editor
{
    [ScriptedImporter(version, extension)]
    internal class AttributeGroupImporter : ScriptedImporter
    {
        private const int version = 1;
        private const string extension = ".attributegroup";
        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            string text;
            try
            {
                text = File.ReadAllText(ctx.assetPath);
            }
            catch (Exception exception)
            {
                ctx.LogImportError($"Could not read file '{ctx.assetPath}' ({exception})");
                return;
            }

            GenerateWrapperClass(ctx, text);

        }

        private void GenerateWrapperClass(AssetImportContext ctx, string fileData)
        {
            var assetPath = ctx.assetPath;
            // Create file at same location as imported asset
            var directory = Path.GetDirectoryName(assetPath);
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var wrapperFilePath = Path.Combine(directory, fileName) + ".cs";

            var sourceFileName = ctx.assetPath;
            var deserializedObj = JsonConvert.DeserializeObject<AttributeGroupFileSchema>(fileData);

            // Generate wrapper class starting at the inner most group for each attribute group
            // Each asset represents a single attribute group


            // Attributes
            var attributeCsFragmentList = new List<string>();
            for (var i = 0; i < deserializedObj.Attributes.Count; i++)
            {
                var attributeCodeGenerator = new AttributeTemplate();
                var fragment = attributeCodeGenerator.Generate(deserializedObj.Attributes[i].AttributeDescription, deserializedObj.Attributes[i].AttributeName);
                attributeCsFragmentList.Add(fragment);
            }

            var attributesFragment = String.Join(Environment.NewLine, attributeCsFragmentList);

            // Attribute Group
            var attributeGroupCodeGen = new AttributeGroupTemplate();
            var attributeGroupFragment = attributeGroupCodeGen.Generate(deserializedObj.Description, deserializedObj.Name, attributesFragment);

            // Base Template
            var baseTemplateCodeGen = new BaseTemplate();
            var classFragment = baseTemplateCodeGen.Generate(version.ToString(), ctx.assetPath, deserializedObj.Namespace, attributeGroupFragment);

            WriteFile(wrapperFilePath, classFragment);


            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        private static void WriteFile(string wrapperFilePath, string content)
        {

            File.WriteAllText(wrapperFilePath, content);
        }


    }

    internal abstract class CodeGenerator
    {
        protected abstract string ReadFile();
        public string Generate(List<(string from, string to)> replaceText)
        {
            var text = ReadFile();
            for (var i = 0; i < replaceText.Count; i++)
            {
                text = text.Replace("{" + replaceText[i].from + "}", replaceText[i].to);
            }

            return text;
        }
    }

    internal class BaseTemplate : CodeGenerator
    {
        public string Generate(string version, string sourcePath, string @namespace, string contents)
        {
            var replaceList = new List<(string from, string to)>
            {
                (from: "VERSION", to: version),
                (from: "SOURCE_PATH", to: sourcePath),
                (from: "NAMESPACE", to: @namespace),
                (from: "CONTENTS", to: contents)
            };

            return ((CodeGenerator)this).Generate(replaceList);
        }

        protected override string ReadFile()
        {
            var fileContents = File.ReadAllText("Packages/com.sjai013.abilitysystem/Editor/attribute-system/CsTemplates/Base.template");
            return fileContents;
        }

    }

    internal class AttributeGroupTemplate : CodeGenerator
    {

        public string Generate(string description, string name, string contents)
        {
            var replaceList = new List<(string from, string to)>
            {
                (from: "DESCRIPTION", to: description),
                (from: "CONTENTS", to: contents),
                (from: "NAME", to: name)
            };

            return ((CodeGenerator)(this)).Generate(replaceList);
        }

        protected override string ReadFile()
        {
            var fileContents = File.ReadAllText("Packages/com.sjai013.abilitysystem/Editor/attribute-system/CsTemplates/AttributeGroup.template");
            return fileContents;
        }
    }

    internal class AttributeTemplate : CodeGenerator
    {

        public string Generate(string description, string name)
        {
            var replaceList = new List<(string from, string to)>
            {
                (from: "DESCRIPTION", to: description),
                (from: "NAME", to: name)
            };

            return ((CodeGenerator)(this)).Generate(replaceList);
        }

        protected override string ReadFile()
        {
            var fileContents = File.ReadAllText("Packages/com.sjai013.abilitysystem/Editor/attribute-system/CsTemplates/Attribute.template");
            return fileContents;
        }
    }
}



//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by Gameplay Ability System - Attribute Group Editor
//     version 1
//     from Assets/Gameplay Ability System DOTS/AttributeSystem/test.attributegroup
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

