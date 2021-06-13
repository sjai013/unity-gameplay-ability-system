using System.Collections;
using System.Collections.Generic;
using GameplayTag.Authoring;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect Definition")]
    public class GameplayEffectScriptableObject : ScriptableObject
    {
        [SerializeField]
        public GameplayEffectDefinitionContainer gameplayEffect;

        [SerializeField]
        public TGameplayEffectTags<GameplayTagScriptableObject> gameplayEffectTags;

        [SerializeField]
        public GameplayEffectPeriod Period;
    }

}
