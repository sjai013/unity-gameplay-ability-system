using System;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects
{
    [Serializable]
    public class GameplayEffectTags : IGameplayEffectTags
    {
        [SerializeField]
        private GameplayEffectTagContainer _assetTags = new GameplayEffectTagContainer();

        [SerializeField]
        private GameplayEffectTagContainer _grantedTags = new GameplayEffectTagContainer();

        [SerializeField]
        private GameplayEffectTagContainer _ongoingTagRequirements = new GameplayEffectTagContainer();

        [SerializeField]
        private GameplayEffectTagContainer _applicationTagRequirements = new GameplayEffectTagContainer();

        [SerializeField]
        private GameplayEffectTagContainer _removeGameplayEffectsWithTag = new GameplayEffectTagContainer();

        public GameplayEffectTagContainer AssetTags => _assetTags;
        public GameplayEffectTagContainer GrantedTags => _grantedTags;
        public GameplayEffectTagContainer OngoingTagRequirements => _ongoingTagRequirements;
        public GameplayEffectTagContainer ApplicationTagRequirements => _applicationTagRequirements;
        public GameplayEffectTagContainer RemoveGameplayEffectsWithTag => _removeGameplayEffectsWithTag;

        
    }

}
