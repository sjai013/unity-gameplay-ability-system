using System.Collections.Generic;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// List of tags that must be present, and list of tags that must not be present
    /// </summary>
    public interface IRequireIgnoreTags {

        List<GameplayTag> RequirePresence { get; }
        List<GameplayTag> RequireAbsence { get; }

    }
}