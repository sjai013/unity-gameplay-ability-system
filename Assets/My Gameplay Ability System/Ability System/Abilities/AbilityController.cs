using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour, DefaultInputActions.IPlayerAbilitiesActions
{
    public AbstractAbilityScriptableObject[] Abilities;

    public AbstractAbilityScriptableObject[] InitialisationAbilities;
    [SerializeField] private AbilitySystemCharacter abilitySystemCharacter;

    private AbstractAbilitySpec[] abilitySpecs;

    private DefaultInputActions playerInput;

    [SerializeField] Animator animatorController;
    public Image[] Cooldowns;

    public Transform[] CastPoint;

    [SerializeField] private CastPointComponent castPointComponent;

    void Awake()
    {
        var spec = Abilities[0].CreateSpec(this.abilitySystemCharacter);
        this.abilitySystemCharacter.GrantAbility(spec);
        playerInput = new DefaultInputActions();
        playerInput.PlayerAbilities.SetCallbacks(this);
        playerInput.PlayerAbilities.Enable();
    }

    void Start()
    {
        ActivateInitialisationAbilities();
        GrantCastableAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < Cooldowns.Length; i++)
        {
            var durationRemaining = this.abilitySpecs[i].CheckCooldown();
            if (durationRemaining.TotalDuration > 0)
            {
                var percentRemaining = durationRemaining.TimeRemaining / durationRemaining.TotalDuration;
                Cooldowns[i].fillAmount = 1 - percentRemaining;
            }
            else
            {
                Cooldowns[i].fillAmount = 1;
            }

        }
    }

    void ActivateInitialisationAbilities()
    {
        for (var i = 0; i < InitialisationAbilities.Length; i++)
        {
            var spec = InitialisationAbilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            StartCoroutine(spec.TryActivateAbility());
        }
    }

    void GrantCastableAbilities()
    {
        this.abilitySpecs = new AbstractAbilitySpec[Abilities.Length];
        for (var i = 0; i < Abilities.Length; i++)
        {
            var spec = Abilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            this.abilitySpecs[i] = spec;
        }
    }

    public void UseAbility(int i)
    {
        var spec = abilitySpecs[i];
        int attackIdx = Random.Range(0, 2);
        if (spec.CanActivateAbility())
        {
            castPointComponent.CastPoint = CastPoint[attackIdx];
            StartCoroutine(spec.TryActivateAbility());
            animatorController.SetInteger("AttackIdx", attackIdx);
            animatorController.SetTrigger("Attack");

        }

    }

    public void OnFire1(InputAction.CallbackContext context)
    {
        UseAbility(0);
    }

    public void OnFire2(InputAction.CallbackContext context)
    {
        UseAbility(1);
    }
}
