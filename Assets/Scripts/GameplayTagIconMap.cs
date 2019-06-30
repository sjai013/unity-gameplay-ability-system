
using System;
using GameplayAbilitySystem;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GameplayTagIconMap", menuName = "Ability System Demo/Gameplay Tag Icon Map")]
public class GameplayTagIconMap : ScriptableObject {
    public GameplayTag Tag;
    public Sprite Sprite;
    public Color SpriteColor;
}