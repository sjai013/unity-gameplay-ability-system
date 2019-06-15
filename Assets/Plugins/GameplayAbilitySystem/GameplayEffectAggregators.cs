using System.Linq;
using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Enums;
using UnityEngine.Events;
using GameplayAbilitySystem.Attributes;

namespace GameplayAbilitySystem.GameplayEffects
{
    public class Aggregator
    {
        private AttributeType AttributeType;

        public Aggregator(AttributeType AttributeType)
        {
            this.AttributeType = AttributeType;
        }

        public Dictionary<EModifierOperationType, List<AggregatorModifier>> Mods { get; } = new Dictionary<EModifierOperationType, List<AggregatorModifier>>();
        public AggregatorEvent Dirtied { get; set; } = new AggregatorEvent();

        public void AddAggregatorMod(float EvaluatedMagnitude, EModifierOperationType ModifierOperation, ActiveGameplayEffectData EffectData)
        {
            // If aggregator exists, check if we have a definition for this modifier operation
            if (!Mods.TryGetValue(ModifierOperation, out var aggregateMods))
            {
                aggregateMods = new List<AggregatorModifier>();
                Mods.Add(ModifierOperation, aggregateMods);
            }
            aggregateMods.Add(new AggregatorModifier(EffectData, EvaluatedMagnitude));
        }

        public void MarkDirty()
        {
            this.OnDirtied();
        }

        protected void OnDirtied()
        {
            Dirtied?.Invoke(this, this.AttributeType);
        }

        public float SumMods(List<AggregatorModifier> Mods)
        {
            return Mods.Sum(x => x.EvaluatedMagnitude);
        }

        public float ProductMods(List<AggregatorModifier> Mods)
        {
            return Mods.Select(x => x.EvaluatedMagnitude).Aggregate((result, item) => result * item);
        }

        public float Evaluate(float BaseValue)
        {
            float additive = 0;
            float multiplicative = 1;
            float divisive = 1;

            if (Mods.TryGetValue(EModifierOperationType.Add, out var AddModifier))
            {
                additive = SumMods(Mods[EModifierOperationType.Add]);
            }

            if (Mods.TryGetValue(EModifierOperationType.Multiply, out var MultiplyModifier))
            {
                multiplicative = ProductMods(Mods[EModifierOperationType.Multiply]);
            }

            if (Mods.TryGetValue(EModifierOperationType.Divide, out var DivideModifier))
            {
                divisive = ProductMods(Mods[EModifierOperationType.Divide]);
            }

            return (BaseValue + additive) * (multiplicative / divisive);
        }

    }

    public class AggregatorModifier
    {
        public AggregatorModifier(ActiveGameplayEffectData ProviderEffectData, float EvaluatedMagnitude, float Stacks = 1)
        {
            this.Stacks = Stacks;
            this.EvaluatedMagnitude = EvaluatedMagnitude;
            this.ProviderEffect = new WeakReference<ActiveGameplayEffectData>(ProviderEffectData);
        }

        public float Stacks { get; private set; }
        public readonly float EvaluatedMagnitude;
        public readonly WeakReference<ActiveGameplayEffectData> ProviderEffect;
    }

    public class AggregatorEvent : UnityEvent<Aggregator, AttributeType>
    {

    }
}

