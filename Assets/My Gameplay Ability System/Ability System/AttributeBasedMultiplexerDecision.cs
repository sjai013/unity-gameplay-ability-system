using AbilitySystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Multpliex/Attribute Based")]
public class AttributeBasedMultiplexerDecision : MultiplexerDecisionScriptableObject
{
    public AttributeSystem.Authoring.AttributeScriptableObject Attribute;
    public float ActivateWhenLessThan;

    public override bool ShouldActivate(AbilitySystemCharacter character)
    {
        if (character.AttributeSystem.GetAttributeValue(Attribute, out var attributeValue))
        {
            if (attributeValue.CurrentValue < ActivateWhenLessThan) return true;
        }
        return false;
    }
}

