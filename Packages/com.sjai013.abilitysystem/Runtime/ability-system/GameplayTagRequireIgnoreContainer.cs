using System;
using GameplayTag.Authoring;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayTagRequireIgnoreContainer<T>
    {
        /// <summary>
        /// All of these tags must be present
        /// </summary>
        public T[] RequireTags;

        /// <summary>
        /// None of these tags can be present
        /// </summary>
        public T[] IgnoreTags;
    }

}
