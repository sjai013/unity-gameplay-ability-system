using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;

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
        private AttributeValue? AttributeValue = null;

        public override void Initialise(GameplayEffectSpec spec)
        {
            AttributeValue = null;
            if (CaptureAttributeWhen == ECaptureAttributeWhen.OnCreation && CaptureAttributeFrom == ECaptureAttributeFrom.Source)
            {
                spec.Source.AttributeSystem.GetAttributeValue(CaptureAttributeWhich, out var sourceAttributeValue);
                this.AttributeValue = sourceAttributeValue;
            }
        }
        public override float? CalculateMagnitude(GameplayEffectSpec spec)
        {
            if (CaptureAttributeWhen == ECaptureAttributeWhen.OnApplication || AttributeValue == null)
            {
                AttributeValue = CaptureAttribute(spec);
            }

            return ScalingFunction.Evaluate(AttributeValue.GetValueOrDefault().CurrentValue);
        }

        private AttributeValue? CaptureAttribute(GameplayEffectSpec spec)
        {
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

    public enum ECaptureAttributeFrom
    {
        Source, Target
    }

    public enum ECaptureAttributeWhen
    {
        OnCreation, OnApplication
    }
}
