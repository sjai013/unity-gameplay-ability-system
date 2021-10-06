using AbilitySystem;
using AbilitySystem.Authoring;
using GameplayAbilitySystemDemo.Input;
using UnityEngine;

public class GrantAbility : MonoBehaviour
{
    private AbilitySystemCharacter m_AbilitySystemCharacter;
    private DefaultInputActions m_InputActions;
    [SerializeField] private AbstractAbility[] ability;

    // Start is called before the first frame update
    void Start()
    {
        this.m_AbilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        m_InputActions = new DefaultInputActions();
        m_InputActions.PlayerAbilities.Enable();
        GrantAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_InputActions.PlayerAbilities.Fire1.triggered)
        {
            if (m_AbilitySystemCharacter.GetGrantedAbilitySpec(ability[1], out var abilitySpec))
            {
                m_AbilitySystemCharacter.ActivateAbility(abilitySpec);
            }
        }

        if (m_InputActions.PlayerAbilities.Fire2.triggered)
        {
            if (m_AbilitySystemCharacter.GetGrantedAbilitySpec(ability[2], out var abilitySpec))
            {
                m_AbilitySystemCharacter.ActivateAbility(abilitySpec);
            }
        }

        if (m_InputActions.PlayerAbilities.Fire3.triggered)
        {
            if (m_AbilitySystemCharacter.GetGrantedAbilitySpec(ability[3], out var abilitySpec))
            {
                m_AbilitySystemCharacter.ActivateAbility(abilitySpec);
            }
        }
    }

    // Grant Abilities
    void GrantAbilities()
    {
        var abilitySpecs = new AbstractAbilitySpec[this.ability.Length];
        for (var i = 0; i < this.ability.Length; i++)
        {
            abilitySpecs[i] = ability[i].CreateSpec(m_AbilitySystemCharacter);
            m_AbilitySystemCharacter.GrantAbility(abilitySpecs[i]);
        }
    }


}
