using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using Unity.Entities;
using UnityEngine;

public class ECSBootstrap : MonoBehaviour {

    private void Start() {
        // get ECS PlayerLoop
        var playerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;

        // set to UniRx.Async PlayerLoop
        PlayerLoopHelper.Initialize(ref playerLoop);

        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity(typeof(PlayerInputComponent));

        entityManager.SetComponentData(entity, new PlayerInputComponent());
    }
}