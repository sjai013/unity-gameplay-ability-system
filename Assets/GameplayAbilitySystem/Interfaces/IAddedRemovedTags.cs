using System.Collections.Generic;
using GameplayAbilitySystem;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// List of added or removed tags.  We only have added tags for now, but maybe we might need removed tags in the future?
    /// </summary>
    public interface IAddedRemovedTags {
        /// <summary>
        /// List of tags to be added
        /// </summary>
        /// <value></value>
        List<GameplayTag> Added { get; }
    }
}