using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayAbilitySystemDemo
{
    public class AbilityCooldownIconUpdater : MonoBehaviour
    {
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Image abilityIconElement;
        [SerializeField] private Sprite abilityIcon;
        private AbilityCooldownValue m_AbilityCooldownValue;

        void Start()
        {
            m_AbilityCooldownValue = GetComponent<AbilityCooldownValue>();
            abilityIconElement.sprite = abilityIcon;
        }
        void Update()
        {
            var cd = m_AbilityCooldownValue.GetCooldownTime();
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