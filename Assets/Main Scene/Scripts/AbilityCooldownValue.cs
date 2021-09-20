using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

namespace GameplayAbilitySystemDemo
{
    public class AbilityCooldownValue : MonoBehaviour
    {
        [SerializeField] private AbilitySystemCharacter m_AbilitySystemCharacter;
        [SerializeField] private AbstractAbility m_Ability;
        [SerializeField] private float m_UpdatePeriod;
        [SerializeField] private AbilityCooldownTime m_CooldownTime;
        private float m_TimeSinceUpdate;

        void Update()
        {
            m_TimeSinceUpdate += Time.deltaTime;
            if (m_TimeSinceUpdate >= m_UpdatePeriod)
            {
                m_TimeSinceUpdate = 0f;
                m_CooldownTime = m_AbilitySystemCharacter.CheckCooldownForTags(m_Ability.Cooldown.GetGameplayEffectTags().GrantedTags);
            }
        }

        public AbilityCooldownTime GetCooldownTime()
        {
            return m_CooldownTime;
        }
    }
}