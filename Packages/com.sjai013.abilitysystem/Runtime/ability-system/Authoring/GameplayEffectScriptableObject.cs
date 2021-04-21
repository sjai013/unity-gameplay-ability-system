using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect Definition")]
    public class GameplayEffectScriptableObject : ScriptableObject
    {
        [SerializeField]
        public GameplayEffectDefinitionContainer gameplayEffect;

        [SerializeField]
        public GameplayEffectTags gameplayEffectTags;

        [SerializeField]
        public GameplayEffectPeriod Period;
    }

}
