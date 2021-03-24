using System;
using GameplayTag.Authoring;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayTagRequireIgnoreContainer
    {
        /// <summary>
        /// All of these tags must be present
        /// </summary>
        public GameplayTagScriptableObject[] RequireTags;

        /// <summary>
        /// None of these tags can be present
        /// </summary>
        public GameplayTagScriptableObject[] IgnoreTags;
    }

}
