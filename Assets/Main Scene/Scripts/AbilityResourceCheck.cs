using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class AbilityResourceCheck : MonoBehaviour
{
    [SerializeField] private float m_UpdatePeriod;
    private AbilitySystemCharacter m_AbilitySystemCharacter;
    private AbstractAbilitySpec m_AbilitySpec;
    private float m_TimeSinceUpdate;

    // Start is called before the first frame update
    void Start()
    {
        var abilityToAbilitySpecComponent = GetComponent<AbilityToAbilitySpec>();
        m_AbilitySystemCharacter = abilityToAbilitySpecComponent.GetAbilitySystemCharacter();
        m_AbilitySpec = abilityToAbilitySpecComponent.GetAbilitySpec();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
