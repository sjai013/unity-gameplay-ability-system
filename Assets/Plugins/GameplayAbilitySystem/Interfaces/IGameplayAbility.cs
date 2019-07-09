using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities.AbilityActivations;
using GameplayAbilitySystem.Events;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayAbilitySystem.Interfaces {
    /// <summary>
    /// This interfaces defines Gameplay Abilities.  Gameplay Abilities represent "things" that players can cast, etc.
    /// E.g. a <see cref="IGameplayAbility"/> might represent a fireball ability which the player casts and which damages a target
    /// </summary>
    public interface IGameplayAbility {
        /// <summary>
        /// Tags that this ability has/provides
        /// </summary>
        /// <value></value>
        IAbilityTags Tags { get; }

        /// <summary>
        /// Cost of using this ability
        /// </summary>
        /// <value></value>
        IGameplayCost GameplayCost { get; }

        /// <summary>
        /// Cooldowns associated with this ability
        /// </summary>
        /// <value></value>
        List<GameplayEffect> CooldownsToApply { get; }

        /// <summary>
        /// This is called whenever this ability ends.
        /// This event does not pass on details of which <see cref="IGameplayAbilitySystem"/> 
        /// was responsible for using this ability.
        /// </summary>
        /// <value></value>
        GenericAbilityEvent OnGameplayAbilityEnded { get; }

        /// <summary>
        /// This is called whenever this ability is commited.
        /// This event does not pass on details of which <see cref="IGameplayAbilitySystem"/> 
        /// was responsible for using this ability.
        /// </summary>
        /// <value></value>
        GenericAbilityEvent OnGameplayAbilityCommitted { get; }

        /// <summary>
        /// This is called whenever this ability is cancelled.
        /// This event does not pass on details of which <see cref="IGameplayAbilitySystem"/> 
        /// was responsible for using this ability.
        /// </summary>
        /// <value></value>
        GenericAbilityEvent OnGameplayAbilityCancelled { get; }

        /// <summary>
        /// Defines what the ability actually does
        /// </summary>
        /// <value></value>        
        AbstractAbilityActivation AbilityLogic { get; }

        /// <summary>
        /// Ends this ability on the target <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="IGameplayAbilitySystem"/></param>
        void EndAbility(IGameplayAbilitySystem AbilitySystem);

        /// <summary>
        /// Activates this ability on the target <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="IGameplayAbilitySystem"/></param>
        void ActivateAbility(IGameplayAbilitySystem AbilitySystem);

        /// <summary>
        /// Check if this ability can be activated by <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="IGameplayAbilitySystem"/></param>
        /// <returns></returns>
        bool IsAbilityActivatable(IGameplayAbilitySystem AbilitySystem);

        /// <summary>
        /// Checks to see if the target <see cref="IGameplayAbilitySystem"/> has the required resources to cast this ability
        /// </summary>
        /// <returns>True if the target <see cref="IGameplayAbilitySystem"/> has required resources, false otherwise</returns>
        bool PlayerHasResourceToCast(IGameplayAbilitySystem AbilitySystem);

        /// <summary>
        /// Checks to see if the <see cref="IGameplayAbilitySystem"/> is off cooldown on the target <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="IGameplayAbilitySystem"/></param>
        /// <returns></returns>
        bool AbilityOffCooldown(IGameplayAbilitySystem AbilitySystem);

        /// <summary>
        /// Commits the <see cref="IGameplayAbilitySystem"/> on the target <see cref="IGameplayAbilitySystem"/>
        /// </summary>
        /// <param name="AbilitySystem">The target <see cref="IGameplayAbilitySystem"/></param>
        /// <returns></returns>
        bool CommitAbility(IGameplayAbilitySystem AbilitySystem);

        /// <summary>
        /// The cooldown tags associated with this <see cref="IGameplayAbility"/>
        /// </summary>
        /// <returns></returns>
        List<GameplayTag> GetAbilityCooldownTags();
        (float CooldownElapsed, float CooldownTotal) CalculateCooldown(IGameplayAbilitySystem AbilitySystem);
    }
}
