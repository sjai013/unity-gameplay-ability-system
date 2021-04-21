using System;
using UnityEngine;

namespace AbilitySystem.Authoring
{

    public abstract class AbstractAbilityScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Name of this ability
        /// </summary>
        [SerializeField] private string AbilityName;

        /// <summary>
        /// Tags for this ability
        /// </summary>
        [SerializeField] public AbilityTags AbilityTags;

        /// <summary>
        /// The GameplayEffect that defines the cost associated with activating the ability
        /// </summary>
        /// <param name="owner">Usually the character activating this ability</param>
        /// <returns></returns>
        [SerializeField] public GameplayEffectScriptableObject Cost;

        /// <summary>
        /// The GameplayEffect that defines the cooldown associated with this ability
        /// </summary>
        /// <param name="owner">Usually the character activating this ability</param>
        /// <returns></returns>
        [SerializeField] public GameplayEffectScriptableObject Cooldown;

        /// <summary>
        /// Creates the Ability Spec (the instantiation of the ability)
        /// </summary>
        /// <param name="owner">Usually the character casting thsi ability</param>
        /// <returns>Ability Spec</returns>
        public abstract AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner);
    }
}