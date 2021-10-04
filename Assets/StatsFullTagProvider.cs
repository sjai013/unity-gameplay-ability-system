using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AttributeSystem.Authoring;
using GameplayTag.Authoring;
using UnityEngine;

public class StatsFullTagProvider : MonoBehaviour, IGameplayTagProvider
{
    [SerializeField] private AttributeScriptableObject HealthAttribute;
    [SerializeField] private AttributeScriptableObject MaxHealthAttribute;
    [SerializeField] private AttributeScriptableObject ManaAttribute;
    [SerializeField] private AttributeScriptableObject MaxManaAttribute;

    [SerializeField] private GameplayTagScriptableObject HealthManaFullTag;

    [SerializeField] private AbilitySystemCharacter m_AbilitySystemCharacter;

    [SerializeField] private List<GameplayTagScriptableObject> ActiveTags = new List<GameplayTagScriptableObject>(2);
    public List<GameplayTagScriptableObject> ListTags()
    {
        ActiveTags.Clear();
        if (CheckAttributeFull(HealthAttribute, MaxHealthAttribute)
         && CheckAttributeFull(ManaAttribute, MaxManaAttribute)
         )
        {
            ActiveTags.Add(HealthManaFullTag);
        }

        return ActiveTags;
    }

    private bool CheckAttributeFull(AttributeScriptableObject attribute, AttributeScriptableObject maxAttribute)
    {
        if (m_AbilitySystemCharacter.AttributeSystem.GetAttributeValue(attribute, out var attributeValue)
            && m_AbilitySystemCharacter.AttributeSystem.GetAttributeValue(maxAttribute, out var maxAttributeValue)
            && attributeValue.CurrentValue >= maxAttributeValue.CurrentValue)
        {
            return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_AbilitySystemCharacter.RegisterTagSource(this);
    }

}
