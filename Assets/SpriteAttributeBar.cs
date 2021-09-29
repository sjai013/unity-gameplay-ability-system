using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AttributeSystem.Authoring;
using UnityEngine;

public class SpriteAttributeBar : MonoBehaviour
{
    [SerializeField] private SingleAttributeValue m_CurrentValue;
    [SerializeField] private SingleAttributeValue m_MaxValue;

    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    private Material m_Material;

    // Start is called before the first frame update
    void Start()
    {
        m_Material = m_SpriteRenderer.material;
        m_SpriteRenderer.material.SetFloat("_Health", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        var health = 0f;
        if (m_CurrentValue.GetValue(out var current) && m_MaxValue.GetValue(out var max))
        {
            health = current.CurrentValue / max.CurrentValue;
            m_Material.SetFloat("_Health", health);
        }

    }
}
