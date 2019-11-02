using System;
using GameplayAbilitySystem.Interfaces;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayEffects {
    [Serializable]
    public class GameplayEffectTags : _IGameplayEffectTags {
        [SerializeField]
        private GameplayEffectAddRemoveTagContainer _assetTags = new GameplayEffectAddRemoveTagContainer();

        [SerializeField]
        private GameplayEffectAddRemoveTagContainer _grantedTags = new GameplayEffectAddRemoveTagContainer();

        [SerializeField]
        private GameplayEffectRequireIgnoreTagContainer _ongoingTagRequirements = new GameplayEffectRequireIgnoreTagContainer();

        [SerializeField]
        private GameplayEffectRequireIgnoreTagContainer _applicationTagRequirements = new GameplayEffectRequireIgnoreTagContainer();

        [SerializeField]
        private GameplayEffectAddRemoveStacksTagContainer _removeGameplayEffectsWithTag = new GameplayEffectAddRemoveStacksTagContainer();

        public GameplayEffectAddRemoveTagContainer AssetTags => _assetTags;
        public GameplayEffectAddRemoveTagContainer GrantedTags => _grantedTags;
        public GameplayEffectRequireIgnoreTagContainer OngoingTagRequirements => _ongoingTagRequirements;
        public GameplayEffectRequireIgnoreTagContainer ApplicationTagRequirements => _applicationTagRequirements;
        public GameplayEffectAddRemoveStacksTagContainer RemoveGameplayEffectsWithTag => _removeGameplayEffectsWithTag;

    }

}
