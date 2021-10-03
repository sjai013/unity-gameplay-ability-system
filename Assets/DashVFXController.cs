using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace GameplayAbilitySystemDemo
{
    public class DashVFXController : MonoBehaviour
    {
        [SerializeField] VisualEffect m_VFX;
        private AbilityCooldownState m_AbilityCooldownValue;

        private int useHotParamId;
        // Start is called before the first frame update
        void Start()
        {
            m_AbilityCooldownValue = GetComponent<AbilityCooldownState>();
            useHotParamId = Shader.PropertyToID("UseHot");

        }

        // Update is called once per frame
        void Update()
        {
            var cd = m_AbilityCooldownValue.GetCooldownTime();
            if (!m_AbilityCooldownValue.AbilitySystemCharacterHasResources() || m_AbilityCooldownValue.GetCooldownTime().TotalDuration > 0)
            {
                m_VFX.SetBool(useHotParamId, false);
            }
            else
            {
                m_VFX.SetBool(useHotParamId, true);
            }
        }
    }
}