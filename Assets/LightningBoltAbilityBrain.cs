using System.Collections;
using System.Collections.Generic;
using AbilitySystem.Authoring;
using MyGameplayAbilitySystem.SampleAbilities;
using MyGameplayAbilitySystem.SampleAbilities.SimplePrefab;
using UnityEngine;

public class LightningBoltAbilityBrain : MonoBehaviour
{
    [SerializeField] private float m_Delay;
    [SerializeField] private float m_Distance;
    [SerializeField] private LayerMask colliderLayer;
    [SerializeField] private GameplayEffect m_TargetGameplayEffect;

    private SimplePrefabAbility.AbilitySpec m_AbilitySpec;

    private float m_TimeToDamage;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeToDamage = m_Delay;
        m_AbilitySpec = GetComponent<SimplePrefabAbilitySpecContext>().AbilitySpec;
        // Get position
        var position = m_AbilitySpec.Owner.GetComponent<CursorTargetLocator>().GetCursorWorldPosition();
        this.transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        m_TimeToDamage -= Time.deltaTime;
        if (m_TimeToDamage <= 0)
        {
            // Place raycast slightly above target, so we can raycast down from it
            var hit = Physics2D.Raycast((Vector2)this.transform.position + new Vector2(0, 1.5f), Vector2.down, m_Distance, colliderLayer);
            if (hit.collider != null)
            {
                var target = hit.collider.gameObject.GetComponent<AbilitySystemTag>()?.Owner;
                if (target == null) return;
                var GE_spec = m_AbilitySpec.Owner.MakeOutgoingSpec(m_TargetGameplayEffect, m_AbilitySpec.Level);

                // Set target
                GE_spec.SetTarget(target);

                // Apply GE
                target.ApplyGameplayEffectSpecToSelf(GE_spec);
            }
            this.enabled = false;
        }

    }
}
