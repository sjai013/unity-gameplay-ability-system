using System;
using System.Collections;
using System.Collections.Generic;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using UnityEngine;
public class HitboxMonoComponent : MonoBehaviour {
    public ActorAbilitySystem ActorAbilitySystem;
    public event EventHandler<ColliderEventArgs> TriggerEnterEvent;

    private void OnTriggerEnter(Collider other) {
        EventHandler<ColliderEventArgs> handler = TriggerEnterEvent;
        handler?.Invoke(this, new ColliderEventArgs()
        {
            @this = this.GetComponent<Collider>(),
            other = other
        });
    }


}
