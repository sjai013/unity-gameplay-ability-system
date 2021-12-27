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

            GenerateWrapperCode(ctx, text);

        }

        private void GenerateWrapperCode(AssetImportContext ctx, string fileData)
        {
            var assetPath = ctx.assetPath;
            // Create file at same location as imported asset
            var directory = Path.GetDirectoryName(assetPath);
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var wrapperFilePath = Path.Combine(directory, fileName) + ".cs";

            var sourceFileName = ctx.assetPath;

            var deserializedObj = JsonConvert.DeserializeObject<AttributeGroupFileSchema>(fileData);
            CodeCompileUnit targetUnit;
            CodeTypeDeclaration attributeGroupClass;

            CreateBaseAttributeGroupClass(deserializedObj, out targetUnit, out attributeGroupClass);
            CreateAttributeClasses(deserializedObj, attributeGroupClass);
            WriteFile(wrapperFilePath, targetUnit);

        }

        private static void WriteFile(string wrapperFilePath, CodeCompileUnit targetUnit)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(wrapperFilePath))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }

        private void CreateAttributeClasses(AttributeGroupFileSchema deserializedObj, CodeTypeDeclaration attributeGroupClass)
        {
            var baseValueMember = CreateIComponentData("BaseValue", "Base Value of Attribute", new List<(Type type, string name, string comment)>() {
               (typeof(float), "Value", "Value of component")
            });

            var currentValueMember = CreateIComponentData("CurrentValue", "Current Value of Attribute", new List<(Type type, string name, string comment)>() {
               (typeof(float), "Value", "Value of component")
            });

            var modifiersMember = CreateIComponentData("ModifierValues", "Current Modifiers on Attribute", new List<(Type type, string name, string comment)>() {
               (typeof(float), "Add", "Total additive modifiers"),
               (typeof(float), "Multiply", "Total multiplicative modifiers")
            });

            for (var i = 0; i < deserializedObj.Attributes.Count; i++)
            {
                // Create nested class for each attribute
                var attributeClass = new CodeTypeDeclaration("Attribute" + deserializedObj.Attributes[i].AttributeName);
                attributeClass.Comments.Add(new CodeCommentStatement($"<summary>{Environment.NewLine} " + deserializedObj.Attributes[i].AttributeDescription + $"{Environment.NewLine} </summary>", true));
                attributeClass.IsClass = true;
                attributeClass.TypeAttributes =
                    TypeAttributes.Public | TypeAttributes.Sealed;
                attributeGroupClass.Members.Add(attributeClass);


                attributeClass.Members.Add(baseValueMember);
                attributeClass.Members.Add(currentValueMember);
                attributeClass.Members.Add(modifiersMember);

            }
        }

        private static void CreateBaseAttributeGroupClass(AttributeGroupFileSchema deserializedObj, out CodeCompileUnit targetUnit, out CodeTypeDeclaration attributeGroupClass)
        {
            targetUnit = new CodeCompileUnit();
            CodeNamespace namespaces = new CodeNamespace("MyGameplayAbilitySystem.Attributes");
            namespaces.Imports.Add(new CodeNamespaceImport("GameplayAbilitySystem.AttributeSystem"));
            attributeGroupClass = new CodeTypeDeclaration("AttributeGroup" + deserializedObj.Name);
            attributeGroupClass.Comments.Add(new CodeCommentStatement($"<summary>{Environment.NewLine} " + deserializedObj.Description + $"{Environment.NewLine} </summary>", true));

            attributeGroupClass.IsClass = true;
            attributeGroupClass.TypeAttributes =
                TypeAttributes.Public | TypeAttributes.Sealed;
            namespaces.Types.Add(attributeGroupClass);
            targetUnit.Namespaces.Add(namespaces);
        }

        private CodeTypeDeclaration CreateIComponentData(string name, string comment, List<(Type type, string name, string comment)> members)
        {
            var s = new CodeTypeDeclaration(name);
            s.IsStruct = true;
            s.TypeAttributes =
TypeAttributes.Public | TypeAttributes.Sealed;
            s.BaseTypes.Add("Unity.Entities.IComponentData");
            s.Comments.Add(new CodeCommentStatement($"<summary>{Environment.NewLine} " + comment + $"{Environment.NewLine} </summary>", true));
            for (var i = 0; i < members.Count; i++)
            {
                var member = new CodeMemberField();
                member.Type = new CodeTypeReference(members[i].type);
                member.Name = members[i].name;
                member.Attributes = MemberAttributes.Public;
                member.Comments.Add(new CodeCommentStatement($"<summary>{Environment.NewLine} " + members[i].comment + $"{Environment.NewLine} </summary>", true));
                s.Members.Add(member);
            }
            return s;
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

