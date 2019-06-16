using System.Linq;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes;
using UniRx.Async;

namespace GameplayAbilitySystem.GameplayEffects
{
    public class ActiveEffectAttributeAggregator
    {
        Dictionary<ActiveGameplayEffectData, Dictionary<AttributeType, Aggregator>> Map = new Dictionary<ActiveGameplayEffectData, Dictionary<AttributeType, Aggregator>>();

        public void RemoveEffect(ActiveGameplayEffectData EffectData)
        {
            this.Map.Remove(EffectData);
        }

        public Dictionary<AttributeType, Aggregator> AddOrGet(ActiveGameplayEffectData EffectData)
        {
            if (!Map.TryGetValue(EffectData, out var attributeAggregatorMap))
            {
                attributeAggregatorMap = new Dictionary<AttributeType, Aggregator>();
                Map.Add(EffectData, attributeAggregatorMap);
            }

            return attributeAggregatorMap;
        }

        public List<ActiveGameplayEffectData> GetActiveEffects() {
            return Map.Keys.ToList();
        }

        public IEnumerable<Aggregator> GetAggregatorsForAttribute(AttributeType Attribute)
        {
            // Find all remaining aggregators of the same type and recompute values
            var aggregators = Map
                                .Where(x => x.Value.ContainsKey(Attribute))
                                .Select(x => x.Value[Attribute]);

            return aggregators;
        }
    }
}

