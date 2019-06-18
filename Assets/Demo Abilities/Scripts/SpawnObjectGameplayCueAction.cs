
using System;
using GameplayAbilitySystem.GameplayCues;
using UniRx.Async;
using UnityEngine;

namespace AbilitySystemDemo {

    [CreateAssetMenu(fileName = "Spawn Object Gameplay Cue", menuName = "Ability System Demo/Gameplay Cue/Spawn Object Gameplay Cue")]
    class SpawnObjectGameplayCueAction : BaseGameplayCueAction {
        public GameObject ObjectToSpawn;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale = Vector3.one;
        public float DestroyInSeconds = -1;
        
        public override async void Action(UnityEngine.GameObject Target, GameplayCueParameters Parameters) {
            Time.timeScale = 0.5f;
            await UniTask.DelayFrame(5);
            Time.timeScale = 1;

            var gameObject = Instantiate(ObjectToSpawn);
            gameObject.transform.SetParent(Target.transform);
            gameObject.transform.localPosition = Position;
            gameObject.transform.localRotation = Rotation;
            gameObject.transform.localScale = Scale;
            if (DestroyInSeconds > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(DestroyInSeconds));
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}