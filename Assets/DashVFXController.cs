using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace GameplayAbilitySystemDemo
{
    public class DashVFXController : MonoBehaviour
    {
        [SerializeField] VisualEffect m_VFX;
        private AbilityCooldownState m_AbilityCooldownValue;

        private int intensityParamId;
        // Start is called before the first frame update
        void Start()
        {
            m_AbilityCooldownValue = GetComponent<AbilityCooldownState>();
            intensityParamId = Shader.PropertyToID("Intensity");

        }

        // Update is called once per frame
        void Update()
        {
            var cd = m_AbilityCooldownValue.GetCooldownTime();
            m_VFX.SetFloat(intensityParamId, cd.TotalDuration == 0 ? 1 : cd.TimeRemaining / cd.TotalDuration);
            if (!m_AbilityCooldownValue.AbilitySystemCharacterHasResources())
            {
                m_VFX.SetFloat(intensityParamId, 0);

            }
        }
    }
}