using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class AbilityToAbilitySpec : MonoBehaviour
{
    [SerializeField] private AbilitySystemCharacter m_AbilitySystemCharacter;
    [SerializeField] private AbstractAbility m_Ability;
    private AbstractAbilitySpec m_AbilitySpec;
    void Start()
    {
        AssignAbilitySpec();
    }

    public bool AssignAbilitySpec()
    {
        return m_AbilitySystemCharacter.GetGrantedAbilitySpec(m_Ability, out m_AbilitySpec);
    }

    public AbstractAbilitySpec GetAbilitySpec()
    {
        if (m_AbilitySpec == null) {
            AssignAbilitySpec();
        }
        
        return m_AbilitySpec;
    }

    public AbilitySystemCharacter GetAbilitySystemCharacter()
    {
        return m_AbilitySystemCharacter;
    }

}
