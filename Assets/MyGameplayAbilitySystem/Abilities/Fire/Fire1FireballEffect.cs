using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Abilities.Components;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using UnityEngine;

public delegate void BasicCastDelegate(ActorAbilitySystem Target);

public class Fire1FireballEffect : MonoBehaviour {
    // Start is called before the first frame update
    public HitboxMonoComponent Fireball;
    public float projectileSpeed = 1f;
    public float maxProjectileDisplacement = 10f;
    public int maxPenetration;
}
