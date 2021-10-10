using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private AbstractAbility StrongAbility;
    [SerializeField] private AbstractAbility WeakAbility;
    [SerializeField] protected AbilitySystemCharacter m_AbilitySystemCharacter;
    void Update()
    {

    }
}