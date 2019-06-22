using System;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues {
    [CreateAssetMenu(fileName = "GameplayCue", menuName = "Ability System/Gameplay Cue/Gameplay Cue")]
    public class GameplayCue : ScriptableObject {

        [SerializeField]
        protected BaseGameplayCueAction ExecuteAction;

        [SerializeField]
        protected BaseGameplayCueAction OnActiveAction;

        [SerializeField]
        protected BaseGameplayCueAction WhileActiveAction;

        [SerializeField]
        protected BaseGameplayCueAction OnRemoveAction;

        public void HandleGameplayCue(GameObject Target, GameplayCueParameters Parameters, EGameplayCueEvent Event) {
            switch (Event) {
                case EGameplayCueEvent.Execute:
                    ExecuteAction.Action(Target, Parameters);
                    break;
                case EGameplayCueEvent.OnActive:
                    OnActiveAction.Action(Target, Parameters);
                    break;
                case EGameplayCueEvent.WhileActive:
                    WhileActiveAction.Action(Target, Parameters);
                    break;
                case EGameplayCueEvent.OnRemove:
                    OnRemoveAction.Action(Target, Parameters);
                    break;
            }
        }
    }


    public enum EGameplayCueEvent {
        Execute, OnActive, WhileActive, OnRemove
    }
}
