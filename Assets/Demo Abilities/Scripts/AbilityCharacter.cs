using System.Collections.Generic;
using GameplayAbilitySystem;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using System;
using Unity.Entities;

public class AbilityCharacter : MonoBehaviour {
    public GameplayAbilitySystem.AbilitySystemComponent SelfAbilitySystem { get; private set; }

    public List<CastingAbilityContainer> Abilities = new List<CastingAbilityContainer>();

    public List<AbilitySystemComponent> TargetPool;

    // Start is called before the first frame update
    void Start() {
        SelfAbilitySystem = GetComponent<GameplayAbilitySystem.AbilitySystemComponent>();

        // Grant all abilities to character, for now
        var em = World.Active.EntityManager;
        foreach (KeyValuePair<EAbility, Type> entry in AbilitySystemComponent.Abilities) {
            var entity = em.CreateEntity();
            em.AddComponent(entity, entry.Value);
            em.AddComponent(entity, typeof(GrantedAbilityComponent));
            em.AddComponent(entity, typeof(GrantedAbilityCooldownComponent));
            em.SetComponentData(entity, new GrantedAbilityComponent()
            {
                GrantedTo = SelfAbilitySystem.entity
            });
        }


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