using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public abstract class MultiplexerDecisionScriptableObject : ScriptableObject
{
    [SerializeField] private AbstractAbilityScriptableObject ability;
    public abstract bool ShouldActivate(AbilitySystemCharacter character);
    public AbstractAbilityScriptableObject GetAbility()
    {
        return this.ability;
    }

    public AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        return this.ability.CreateSpec(owner);
    }
};

