using GameplayAbilitySystem.Abilities.AbilityActivations;
using GameplayAbilitySystem.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using Unity.Entities;
using UnityEngine;
using UnityStandardAssets.Cameras;


[CreateAssetMenu(fileName = "TargettingLogic", menuName = "Ability System Demo/Ability Logic/Targetting Logic/Area")]
public class AreaTargetting : AbstractTargettingLogic {

    public override List<IGameplayAbilitySystem> InitiateTargetting(IGameplayAbilitySystem AbilitySystem, IGameplayAbility Ability) {

        return new List<IGameplayAbilitySystem>();
    }

}

