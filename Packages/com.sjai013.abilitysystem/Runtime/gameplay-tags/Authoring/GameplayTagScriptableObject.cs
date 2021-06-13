using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;
using UnityEngine.Serialization;

namespace GameplayTag.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Tag")]
    public class GameplayTagScriptableObject : ScriptableObject
    {
        [SerializeField] private GameplayTagScriptableObject Parent;
        [SerializeField] private int ancestorsToFind = 4;
        public GameplayTag TagData;

        /// <summary>
        /// <para>Check is this gameplay tag is a descendant of another gameplay tag.</para>
        /// By default, only 4 levels of ancestors are searched.
        /// </summary>
        /// <param name="other">Ancestor gameplay tag</param>
        /// <returns>True if this gameplay tag is a descendant of the other gameplay tag</returns>
        public bool IsDescendantOf(GameplayTagScriptableObject other, int nSearchLimit = 4)
        {
            int i = 0;
            GameplayTagScriptableObject tag = Parent;
            while (nSearchLimit > i++)
            {
                // tag will be invalid once we are at the root ancestor
                if (!tag) return false;

                // Match found, so we can return true
                if (tag == other) return true;

                // No match found, so try again with the next ancestor
                tag = tag.Parent;
            }


            // If we've exhausted the search limit, no ancestor was found
            return false;
        }

        public void OnValidate()
        {
            UpdateCache();
        }

        private void UpdateCache()
        {
            this.TagData = Build(ancestorsToFind);
        }

        public GameplayTag Build(int nSearchLimit = 4)
        {
            if (nSearchLimit < 0) nSearchLimit = ancestorsToFind;

            var ancestors = new List<int>();
            var parent = this.Parent;
            for (var i = 0; i < nSearchLimit; i++)
            {
                ancestors.Add(parent?.GetInstanceID() ?? 0);
                // Leave the loop early if there no further ancestors
                parent = parent?.Parent;
                i = math.select(i, nSearchLimit, parent == null);
            }

            return new GameplayTag()
            {
                Tag = this.GetInstanceID(),
                Ancestors = ancestors.ToArray()
            };
        }

        [Serializable]
        public struct GameplayTag
        {
            public int Tag;
            public int[] Ancestors;

            public bool IsDescendantOf(GameplayTag other)
            {
                return (other.Ancestors.Contains(Tag));
            }

            public override bool Equals(object obj)
            {
                return obj is GameplayTag && this == (GameplayTag)obj;
            }

            public override int GetHashCode()
            {
                return Tag.GetHashCode();
            }
            public static bool operator ==(GameplayTag x, GameplayTag y)
            {
                return x.Tag == y.Tag;
            }

            public static bool operator !=(GameplayTag x, GameplayTag y)
            {
                return !(x == y);
            }
        }
    }
}
