using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameplayAbilitySystem.ExtensionMethods;
using GameplayAbilitySystem.Statics;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using GameplayAbilitySystem.Events;
using UniRx.Async;
using UnityEngine;
using System;

public class AbilityCharacter : MonoBehaviour {
    public GameplayAbilitySystem.AbilitySystemComponent SelfAbilitySystem { get; private set; }

    public List<CastingAbilityContainer> Abilities = new List<CastingAbilityContainer>();

    public List<AbilitySystemComponent> TargetPool;

    // Start is called before the first frame update
    void Start() {
        SelfAbilitySystem = GetComponent<GameplayAbilitySystem.AbilitySystemComponent>();
    }

    public void CastAbility(int n) {
        if (n >= this.Abilities.Count) return;
        if (this.Abilities[n] == null) return;
        if (this.Abilities[n].Ability == null) return;

        var Target = this.Abilities[n].AbilityTarget;

        //Randomise Target
        if (Target == null) {
            var randomIndex = UnityEngine.Random.Range(0, TargetPool.Count);
            Target = TargetPool[randomIndex];
        }

        // If ability can be activated
        SelfAbilitySystem.TryActivateAbility(this.Abilities[n].AbilityType, this.SelfAbilitySystem, Target);
    }

}

[Serializable]
public class CastingAbilityContainer {
    public GameplayAbility Ability;
    public EAbility AbilityType;
    public GameplayAbilitySystem.AbilitySystemComponent AbilityTarget;
}