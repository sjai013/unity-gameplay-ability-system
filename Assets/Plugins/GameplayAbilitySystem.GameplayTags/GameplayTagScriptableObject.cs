using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace GamplayAbilitySystem.GameplayTags
{
    [CreateAssetMenu(fileName = "GameplayTag", menuName = "Gameplay Ability System/Gameplay Tags/Gameplay Tag", order = 1)]
    public class GameplayTagScriptableObject : ScriptableObject
    {
        public string GameplayTagString;

        [SerializeField]
        public GameplayTag Tag;


        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct GameplayTag
        {
            [FieldOffset(0)]
            private int Id;

            [FieldOffset(0)]
            public byte L0;

            [FieldOffset(1)]
            public byte L1;

            [FieldOffset(2)]
            public byte L2;

            [FieldOffset(3)]
            public byte L3;
        }
    }

    public class GameplayTagIdAssigner
    {
        [MenuItem("Assets/Test")]
        private static void Test()
        {
            GameplayTagScriptableObject[] selectedAsset = Selection.GetFiltered<GameplayTagScriptableObject>(SelectionMode.DeepAssets);
            if (!EditorUtility.DisplayDialog("Auto Generate Gameplay Tag ID?", "Auto generate IDs for all GameplayTag assets in this folder/subfolders?", "OK", "Cancel"))
                return;

            List<List<string>> Tags = new List<List<string>>() { new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            foreach (GameplayTagScriptableObject obj in selectedAsset)
            {
                if (obj.GameplayTagString == "")
                {
                    obj.GameplayTagString = obj.name;
                }

                var ids = obj.GameplayTagString.Split('.');
                byte[] tagIds = new byte[4];

                var scriptableObject = (GameplayTagScriptableObject)obj;
                for (var i = 0; i < ids.Length; i++)
                {
                    if (i >= 4) break;
                    var tagString = ids[i];

                    // Check if this string already exists at this level
                    var existingIndex = Tags[i].IndexOf(tagString);
                    if (existingIndex >= 0)
                    {
                        tagIds[i] = (byte)(existingIndex + 1);
                    }
                    else
                    {
                        Tags[i].Add(tagString);
                        // We want the just added index +1
                        tagIds[i] = (byte)(Tags[i].Count);
                    }
                }
                scriptableObject.Tag.L0 = tagIds[0];
                scriptableObject.Tag.L1 = tagIds[1];
                scriptableObject.Tag.L2 = tagIds[2];
                scriptableObject.Tag.L3 = tagIds[3];
            }
        }
    }

}