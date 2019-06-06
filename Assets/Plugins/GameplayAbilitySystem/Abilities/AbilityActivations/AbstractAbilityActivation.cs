using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.Abilities.AbilityActivations
{
    public abstract class AbstractAbilityActivation : ScriptableObject
    {
        public abstract void ActivateAbility(IGameplayAbilitySystem AbilitySystem, IGameplayAbility Ability);
    }
}