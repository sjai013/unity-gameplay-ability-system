using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using GameplayAbilitySystemDemo.Input;
using UnityEngine;

public class UseAbility : MonoBehaviour
{
    private AbilitySystemCharacter abilitySystemCharacter;
    private DefaultInputActions m_InputActions;
    [SerializeField] private AbstractAbility ability;



    // Start is called before the first frame update
    void Start()
    {
        this.abilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        m_InputActions = new DefaultInputActions();
        m_InputActions.PlayerAbilities.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        if (m_InputActions.PlayerAbilities.Fire1.triggered) {
            abilitySystemCharacter.ActivateAbility(ability.CreateSpec(abilitySystemCharacter));
        }
    }
}
