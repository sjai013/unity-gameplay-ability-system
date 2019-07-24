using System.Collections.Generic;
using UnityEngine;
using GameplayAbilitySystem.Interfaces;

namespace GameplayAbilitySystem.Abilities.AbilityActivations {

    public abstract class AbstractTargettingLogic : ScriptableObject {

        public abstract List<IGameplayAbilitySystem> InitiateTargetting(IGameplayAbilitySystem AbilitySystem, IGameplayAbility Ability);
    }
}