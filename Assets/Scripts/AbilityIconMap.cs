
using System;
using GameplayAbilitySystem.Abilities;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "AbilityIconMap", menuName = "Ability System Demo/Ability Icon Map")]
public class AbilityIconMap : ScriptableObject
{
    public GameplayAbility Ability;
    public Sprite Sprite;
    public Color SpriteColor;
}