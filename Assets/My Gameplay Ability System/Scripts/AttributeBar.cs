using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    [SerializeField] private SingleAttributeValue m_CurrentValue;
    [SerializeField] private SingleAttributeValue m_MaxValue;
    [SerializeField] private Gradient m_Color;
    [SerializeField] private bool m_Lerp = true;
    private Image m_Image;
    [SerializeField] private float m_LerpSpeed = 2f;
    void Start()
    {
        if (m_MaxValue == null)
        {
            m_CurrentValue = m_MaxValue;
        }

        m_Image = GetComponent<Image>();

    }

    void Update()
    {
        if (m_CurrentValue == null || m_MaxValue == null) return;

        if (m_CurrentValue.GetValue(out var currentValue) == true
            && m_MaxValue.GetValue(out var maxValue) == true)
        {
            var valuePercent = math.clamp(currentValue.CurrentValue / maxValue.CurrentValue, 0, 1);
            var previousValue = m_Image.fillAmount;

            m_Image.color = m_Color.Evaluate(valuePercent);

            if (m_Lerp)
            {
                valuePercent = math.lerp(previousValue, valuePercent, Time.deltaTime * m_LerpSpeed);
            }

            m_Image.fillAmount = valuePercent;
        }
    }
}
