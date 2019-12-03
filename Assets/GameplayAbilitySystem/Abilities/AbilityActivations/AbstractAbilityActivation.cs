using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Interfaces;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.AbilityActivations {

    public abstract class AbstractAbilityActivation : ScriptableObject {

        public abstract void ActivateAbility(AbilitySystemComponent AbilitySystem, GameplayAbility Ability);

        public abstract void ActivateAbility(AbilitySystemComponent Source, AbilitySystemComponent Target, _IAbilityBehaviour Ability);
    }
}