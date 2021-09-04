using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class AwakeAbility : MonoBehaviour
{
    [SerializeField] private AbstractAbility[] m_Abilities;
    private AbilitySystemCharacter abilitySystemCharacter;

    // Start is called before the first frame update
    void Start()
    {
        abilitySystemCharacter = this.GetComponent<AbilitySystemCharacter>();
        for (var i = 0; i < m_Abilities.Length; i++)
        {
            AbstractAbilitySpec abilitySpec = m_Abilities[i].CreateSpec(abilitySystemCharacter);
            abilitySystemCharacter.ActivateAbility(abilitySpec);

        }
    }


}
