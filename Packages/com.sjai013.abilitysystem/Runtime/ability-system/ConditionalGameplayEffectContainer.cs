using System;
using AbilitySystem.Authoring;
using GameplayTag.Authoring;

namespace AbilitySystem
{
    [Serializable]
    public struct ConditionalGameplayEffectContainer
    {
        public GameplayEffect GameplayEffect;
        public GameplayTagScriptableObject[] RequiredSourceTags;
    }

}
