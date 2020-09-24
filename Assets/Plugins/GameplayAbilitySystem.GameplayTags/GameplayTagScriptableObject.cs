using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
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
            private uint Id;

            [FieldOffset(0)]
            public byte L0;

            [FieldOffset(1)]
            public byte L1;

            [FieldOffset(2)]
            public byte L2;

            [FieldOffset(3)]
            public byte L3;


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsEqualTo(GameplayTag other)
            {
                // Apply bitmask based on this
                // e.g. for A.B.C.D, this A matches other A.*
                var mask = 0xFFFFFFFF;
                mask &= L3 == 0 ? 0xFFFFFF00 : 0xFFFFFFFF;
                mask &= L2 == 0 ? 0xFFFF0000 : 0xFFFFFFFF;
                mask &= L1 == 0 ? 0xFF000000 : 0xFFFFFFFF;
                mask &= L0 == 0 ? 0x00000000 : 0xFFFFFFFF;
                return (mask & other.Id) == Id;
            }
        }


    }

    public class GameplayTagIdAssigner
    {
        [MenuItem("Assets/Generate Tag IDs")]
        private static void GenerateTagIDs()
        {
            if (!EditorUtility.DisplayDialog("Auto Generate Gameplay Tag ID?", "Auto generate IDs for all GameplayTag assets in this folder and all subfolders?", "OK", "Cancel"))
                return;

            // Assign name of SO to internal string for debugging
            GameplayTagScriptableObject[] selectedAsset = Selection.GetFiltered<GameplayTagScriptableObject>(SelectionMode.DeepAssets);
            foreach (var asset in selectedAsset)
            {
                if (asset.GameplayTagString == "")
                {
                    asset.GameplayTagString = asset.name;
                }
            }

            // Order by name
            selectedAsset = selectedAsset.OrderBy(x => x.GameplayTagString).ToArray();

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