using System;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues {
    [CreateAssetMenu(fileName="GameplayCue", menuName="Ability System/Gameplay Cue/Gameplay Cue")]
    public class GameplayCue : ScriptableObject {

        [SerializeField]
        protected BaseGameplayCueAction ExecuteAction;

        [SerializeField]
        protected BaseGameplayCueAction OnActiveAction;

        [SerializeField]
        protected BaseGameplayCueAction WhileActiveAction;

        [SerializeField]
        protected BaseGameplayCueAction OnRemoveAction;

        public void HandleGameplayCue_Execute(GameObject Target, GameplayCueParameters Parameters) {
            ExecuteAction.Action(Target, Parameters);
        }
        public void HandleGameplayCue_OnActive(GameObject Target, GameplayCueParameters Parameters) {
            OnActiveAction.Action(Target, Parameters);
        }
        public void HandleGameplayCue_WhileActive(GameObject Target, GameplayCueParameters Parameters) {
            WhileActiveAction.Action(Target, Parameters);
        }
        public void HandleGameplayCue_OnRemove(GameObject Target, GameplayCueParameters Parameters) {
            OnRemoveAction.Action(Target, Parameters);
        }
    }
}
