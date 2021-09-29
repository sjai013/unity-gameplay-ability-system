using System;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    internal enum EGameplayCueTarget
    {
        Source, Target
    }

    internal enum EGameplayCueRemovePolicy
    {
        Never, WithGameplayEffect, AfterDelay
    }

    internal enum EGameplayCueApplyPolicy
    {
        Never, OnGameplayEffectApply, OnTick
    }

    [Serializable]
    internal struct PrefabGameplayCueParameters
    {
        public GameObject OnActivateEffect;
        public EGameplayCueTarget Target;
        public EGameplayCueApplyPolicy ApplyPolicy;
        public EGameplayCueRemovePolicy RemovePolicy;
        public float RemoveDelay;
    }

    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Cue/Prefab")]
    public class PrefabGameplayCue : GameplayCueScriptableObject
    {
        [SerializeField] private PrefabGameplayCueParameters m_GameplayCueParameters;

        public override AbstractGameplayCueSpec CreateSpec(GameplayEffectSpec geSpec)
        {
            var gcSpec = new GameplayCueSpec(geSpec);
            gcSpec.SetGameplayCueParameters(m_GameplayCueParameters);
            gcSpec.Initialise(geSpec);
            return gcSpec;
        }

        public class GameplayCueSpec : GameplayCueScriptableObject.AbstractGameplayCueSpec
        {
            private GameObject gameObject;
            private PrefabGameplayCueParameters gcParams;
            public GameplayCueSpec(GameplayEffectSpec spec) : base(spec)
            {
            }

            internal void SetGameplayCueParameters(PrefabGameplayCueParameters gcParams)
            {
                this.gcParams = gcParams;
            }

            public override void Initialise(GameplayEffectSpec spec)
            {
                switch (gcParams.ApplyPolicy)
                {
                    case EGameplayCueApplyPolicy.OnGameplayEffectApply:
                        spec.OnApply += (s) => InstantiatePrefab(s);
                        break;
                    case EGameplayCueApplyPolicy.OnTick:
                        spec.OnTick += (s) =>
                        {
                            InstantiatePrefab(s);
                        };
                        break;
                }

            }

            public override void Activate(GameplayEffectSpec spec)
            {



            }

            private GameObject InstantiatePrefab(GameplayEffectSpec spec)
            {
                var targetTransform = GetPosition(gcParams.Target, spec);
                gameObject = Instantiate(gcParams.OnActivateEffect, targetTransform.position, Quaternion.identity);
                gameObject.transform.SetParent(targetTransform);
                switch (gcParams.RemovePolicy)
                {
                    case EGameplayCueRemovePolicy.WithGameplayEffect:
                        spec.OnRemove += (s) => Remove(s);
                        break;
                    case EGameplayCueRemovePolicy.AfterDelay:
                        Remove(spec);
                        break;
                }

                return gameObject;
            }

            public override void Remove(GameplayEffectSpec spec)
            {
                Destroy(gameObject, gcParams.RemoveDelay);
            }

            private Transform GetPosition(EGameplayCueTarget m_Target, GameplayEffectSpec spec)
            {
                switch (m_Target)
                {
                    case EGameplayCueTarget.Source:
                        return spec.Source.transform;
                    case EGameplayCueTarget.Target:
                        return spec.Target.transform;
                    default:
                        return null;
                }
            }

        }
    }

}