using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private AbstractAbilityScriptableObject initialStats;

    [SerializeField]
    private AbilitySystemCharacter asc;
    void Start()
    {
        var spec = initialStats.CreateSpec(asc);
        asc.GrantAbility(spec);
        StartCoroutine(spec.TryActivateAbility());

    }

    // Update is called once per frame
    void Update()
    {

    }
}
