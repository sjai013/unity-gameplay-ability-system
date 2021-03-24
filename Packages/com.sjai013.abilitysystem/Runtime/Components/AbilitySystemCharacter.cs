using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;


namespace AbilitySystem
{
    public class AbilitySystemCharacter : MonoBehaviour
    {
        public AttributeSystemComponent AttributeSystem;
        public List<GameplayEffectSpec> AppliedGameplayEffects;

        public void ApplyGameplayEffect()
        {

        }

        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, int level = 1)
        {

            return new GameplayEffectSpec()
            {
                GameplayEffect = GameplayEffect,
                Duration = GameplayEffect.gameplayEffect.Duration,
                Level = level,
                Period = GameplayEffect.Period

            };
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
