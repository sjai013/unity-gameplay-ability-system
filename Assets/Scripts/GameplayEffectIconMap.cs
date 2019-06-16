
using System;
using GameplayAbilitySystem.GameplayEffects;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "GameplayEffectIconMap", menuName = "Ability System Demo/Gameplay Effect Icon Map")]
public class GameplayEffectIconMap : ScriptableObject
{
    public GameplayEffect GameEffect;
    public Sprite Sprite;
    public Color SpriteColor;
}