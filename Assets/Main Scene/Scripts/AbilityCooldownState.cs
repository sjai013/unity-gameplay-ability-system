using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

namespace GameplayAbilitySystemDemo
{
    public class AbilityCooldownState : MonoBehaviour
    {
        [SerializeField] private float m_UpdatePeriod;
        [SerializeField] private AbilityCooldownTime m_CooldownTime;
        [SerializeField] private bool m_HasResources;

        private AbilitySystemCharacter m_AbilitySystemCharacter;
        private float m_TimeSinceUpdate;

        private AbilityToAbilitySpec abilityToAbilitySpecComponent;

        void Start()
        {
            abilityToAbilitySpecComponent = GetComponent<AbilityToAbilitySpec>();
            m_AbilitySystemCharacter = abilityToAbilitySpecComponent.GetAbilitySystemCharacter();
        }



        void Update()
        {
            if (m_AbilitySystemCharacter == null) return;
            var abilitySpec = abilityToAbilitySpecComponent.GetAbilitySpec();

            if (abilitySpec == null) return;

            m_TimeSinceUpdate += Time.deltaTime;
            if (m_TimeSinceUpdate >= m_UpdatePeriod)
            {
                m_TimeSinceUpdate = 0f;
                m_CooldownTime = m_AbilitySystemCharacter.CheckCooldownForTags(abilitySpec.Ability.Cooldown.GetGameplayEffectTags().GrantedTags);
                m_HasResources = abilitySpec.CheckCost();
            }
        }

        public AbilityCooldownTime GetCooldownTime()
        {
            return m_CooldownTime;
        }

        public bool AbilitySystemCharacterHasResources()
        {
            return m_HasResources;
        }
    }
}