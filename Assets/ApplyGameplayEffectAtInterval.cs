using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ApplyGameplayEffectAtInterval : MonoBehaviour
{
    [SerializeField] private GameplayEffect m_GameplayEffect;
    [SerializeField] private float m_Interval;
    [SerializeField] private float m_TimeToApply;

    private HashSet<AbilitySystemCharacter> m_AscInRegion = new HashSet<AbilitySystemCharacter>(5);
    private BoxCollider2D m_Col;
    private int abilitySystemLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        m_Col = GetComponent<BoxCollider2D>();
        abilitySystemLayerMask = LayerMask.NameToLayer("Ability System Tag");
        m_TimeToApply = m_Interval;
    }

    // Update is called once per frame
    void Update()
    {
        m_TimeToApply -= Time.deltaTime;
        if (m_TimeToApply <= 0)
        {
            m_TimeToApply = m_Interval;
            foreach (var asc in m_AscInRegion)
            {
                var spec = asc.MakeOutgoingSpec(m_GameplayEffect, 1);
                asc.ApplyGameplayEffectSpecToSelf(spec);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == abilitySystemLayerMask)
        {
            var ascTag = other.GetComponent<AbilitySystemTag>();
            if (ascTag != null)
            {
                m_AscInRegion.Add(ascTag.Owner);
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        var ascTag = other.GetComponent<AbilitySystemTag>();
        if (ascTag != null)
        {
            m_AscInRegion.Remove(ascTag.Owner);
        }
    }


}
