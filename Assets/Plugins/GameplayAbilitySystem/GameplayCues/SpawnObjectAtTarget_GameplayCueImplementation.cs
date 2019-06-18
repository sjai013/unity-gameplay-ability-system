using System;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace GameplayAbilitySystem.GameplayCues {
    [CreateAssetMenu(fileName = "SpawnObjectAtTarget", menuName = "Ability System/Gameplay Cue/Spawn Object At Target")]
    public class SpawnObjectAtTarget_GameplayCueImplementation : AbstractGameplayCueImplementation {
        public GameObject ObjectToSpawn;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale = Vector3.one;
        public float DestroyInSeconds = -1;
        public override async void HandleGameplayCue(GameObject Target, EGameplayCueEventTypes EventType, GameplayCueParameters Parameters) {
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
