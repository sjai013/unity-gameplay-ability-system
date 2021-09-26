using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayAbilitySystemDemo
{
    public class AbilityCooldownTextUpdater : MonoBehaviour
    {
        private TextMeshProUGUI m_TextMeshPro;
        private AbilityCooldownState m_AbilityCooldownValue;

        void Start()
        {
            m_TextMeshPro = GetComponent<TextMeshProUGUI>();
            m_AbilityCooldownValue = GetComponent<AbilityCooldownState>();
        }

        void Update()
        {
            var cd = m_AbilityCooldownValue.GetCooldownTime();
            this.m_TextMeshPro.text = cd.TimeRemaining.ToString("0.00") + "/" + cd.TotalDuration.ToString("0.00");

            if (cd.TotalDuration <= 0)
            {
                this.m_TextMeshPro.text = "-";
            }
        }
    }
}