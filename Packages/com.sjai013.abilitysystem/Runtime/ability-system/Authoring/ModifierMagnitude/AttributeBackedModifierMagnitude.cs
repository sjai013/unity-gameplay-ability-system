using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;
using UnityEngine.Events;

namespace AbilitySystem.ModifierMagnitude
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect/Modifier Magnitude/Attribute Backed")]
    public class AttributeBackedModifierMagnitude : ModifierMagnitudeScriptableObject
    {


        [SerializeField]
        private AnimationCurve ScalingFunction;

        [SerializeField]
        private AttributeScriptableObject CaptureAttributeWhich;

        [SerializeField]
        private ECaptureAttributeFrom CaptureAttributeFrom;

        [SerializeField]
        private ECaptureAttributeWhen CaptureAttributeWhen;


        public override void Initialise(GameplayEffectSpec spec)
        {
            spec.Source.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var sourceAttributeValue);
            spec.SourceCapturedAttribute = sourceAttributeValue;
        }

        public override float? CalculateMagnitude(GameplayEffectSpec spec)
        {

            return ScalingFunction.Evaluate(GetCapturedAttribute(spec).GetValueOrDefault().CurrentValue);
        }

        private AttributeValue? GetCapturedAttribute(GameplayEffectSpec spec)
        {
            if (CaptureAttributeWhen == ECaptureAttributeWhen.OnApplication && CaptureAttributeFrom == ECaptureAttributeFrom.Source)
            {
                return spec.SourceCapturedAttribute;
            }

            switch (CaptureAttributeFrom)
            {
                case ECaptureAttributeFrom.Source:
                    spec.Source.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var sourceAttributeValue);
                    return sourceAttributeValue;
                case ECaptureAttributeFrom.Target:
                    spec.Target.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var targetAttributeValue);
                    return targetAttributeValue;
                default:
                    return null;
            }
        }
    }
}
