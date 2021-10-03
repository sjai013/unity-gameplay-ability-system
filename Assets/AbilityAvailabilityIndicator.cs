using System.Collections;
using UnityEngine;

namespace GameplayAbilitySystemDemo
{
    public class AbilityAvailabilityIndicator : MonoBehaviour
    {
        [SerializeField] GameObject m_ObjectToToggle;
        private AbilityCooldownState m_AbilityCooldownValue;


        // Start is called before the first frame update
        void Start()
        {
            m_AbilityCooldownValue = GetComponent<AbilityCooldownState>();

        }

        // Update is called once per frame
        void Update()
        {
            var cd = m_AbilityCooldownValue.GetCooldownTime();
            if (!m_AbilityCooldownValue.AbilitySystemCharacterHasResources() || m_AbilityCooldownValue.GetCooldownTime().TotalDuration > 0)
            {
                m_ObjectToToggle.SetActive(false);
            }
            else
            {
                m_ObjectToToggle.SetActive(true);
            }
        }
    }
}