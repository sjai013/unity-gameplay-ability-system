using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ApplyGameplayEffectOnEnter : MonoBehaviour
{
    [SerializeField] private GameplayEffect m_GameplayEffect;
    [SerializeField] private float m_Level;
    private Dictionary<AbilitySystemCharacter, GameplayEffectSpec> m_CachedSpecs = new(5);
    private BoxCollider2D m_Col;
    private int abilitySystemLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        m_Col = GetComponent<BoxCollider2D>();
        abilitySystemLayerMask = LayerMask.NameToLayer("Ability System Tag");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == abilitySystemLayerMask)
        {
            var ascTag = other.GetComponent<AbilitySystemTag>();
            if (ascTag != null)
            {
                var asc = ascTag.Owner;
                if (!m_CachedSpecs.TryGetValue(asc, out var spec))
                {
                    spec = asc.MakeOutgoingSpec(m_GameplayEffect, 1);
                    asc.ApplyGameplayEffectSpecToSelf(spec);
                    m_CachedSpecs.Add(asc, spec);
                }
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        var ascTag = other.GetComponent<AbilitySystemTag>();
        if (ascTag != null)
        {
            var asc = ascTag.Owner;
            if (m_CachedSpecs.TryGetValue(asc, out var spec))
            {
                asc.RemoveGameplayEffect(spec);
                m_CachedSpecs.Remove(asc);
            }
        }
    }


}
