using System.Collections.Generic;
using GameplayTag.Authoring;


namespace AbilitySystem
{
    public interface IGameplayTagProvider
    {
        List<GameplayTagScriptableObject> ListTags();
    }
}
