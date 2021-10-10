using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ApplyGameplayEffectOnEnter : MonoBehaviour
{
    [SerializeField] private GameplayEffect[] m_GameplayEffects;
    [SerializeField] private float m_Level;
    [SerializeField] private bool RemoveOnExit;
    private Dictionary<AbilitySystemCharacter, GameplayEffectSpec[]> m_CachedSpecs = new(5);
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

                if (!m_CachedSpecs.TryGetValue(asc, out var specs)) // Cache exists, check validity
                {
                    specs = new GameplayEffectSpec[m_GameplayEffects.Length];
                }

                for (var i = 0; i < m_GameplayEffects.Length; i++) // If any of the specs is invalid, apply it now.  Assume length if always fixed
                {
                    if (!asc.HasActiveGameplayEffect(specs[i]))
                    {
                        var spec = asc.MakeOutgoingSpec(m_GameplayEffects[i], this.m_Level);
                        asc.ApplyGameplayEffectSpecToSelf(spec);
                        specs[i] = spec;
                    }
                }
            }
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!RemoveOnExit) return;
        var ascTag = other.GetComponent<AbilitySystemTag>();
        if (ascTag != null)
        {
            var asc = ascTag.Owner;
            if (m_CachedSpecs.TryGetValue(asc, out var specs))
            {
                asc.RemoveActiveGameplayEffect(specs);
                m_CachedSpecs.Remove(asc);
            }
        }
    }


}
