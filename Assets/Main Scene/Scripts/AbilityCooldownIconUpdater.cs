using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayAbilitySystemDemo
{
    public class AbilityCooldownIconUpdater : MonoBehaviour
    {
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Image abilityIconElement;
        [SerializeField] private Image m_BorderIconElement;
        [SerializeField] private Sprite abilityIcon;
        private AbilityCooldownState m_AbilityCooldownValue;

        void Start()
        {
            m_AbilityCooldownValue = GetComponent<AbilityCooldownState>();
            abilityIconElement.sprite = abilityIcon;
        }

        void Update()
        {
            var cd = m_AbilityCooldownValue.GetCooldownTime();

            if (m_AbilityCooldownValue.AbilitySystemCharacterHasResources())
            {
                m_BorderIconElement.color = Color.green;
            }
            else
            {
                m_BorderIconElement.color = Color.red;
            }
            
            if (cd.TotalDuration <= 0)
            {
                cooldownOverlay.fillAmount = 0f;
                return;
            }

            if (cd.TotalDuration > 0)
            {
                cooldownOverlay.fillAmount = cd.TimeRemaining / cd.TotalDuration;
            }


        }
    }
}