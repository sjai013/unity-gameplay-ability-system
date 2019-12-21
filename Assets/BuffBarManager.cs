using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.ScriptableObjects;
using GameplayAbilitySystem.GameplayEffects.Components;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class BuffBarManager : MonoBehaviour {
    public BuffsContainerScriptableObject EffectsToShow;
    public List<GameplayTagStatusBarButton> BuffIcons;
    public ActorAbilitySystem AbilitySystem;
    // Start is called before the first frame update
    void Start() {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuffBarUpdateSystem>().EffectsComponentsToShow = EffectsToShow.ComponentTypes.ToArray();
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuffBarUpdateSystem>().Player = AbilitySystem.AbilityOwnerEntity;
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuffBarUpdateSystem>().BuffIcons = BuffIcons;
    }

    // Update is called once per frame
    void Update() {

    }
}

public class BuffBarUpdateSystem : ComponentSystem {
    public ComponentType[] EffectsComponentsToShow;
    public Entity Player;
    EntityQuery m_Query;
    public List<GameplayTagStatusBarButton> BuffIcons;
    protected override void OnStartRunning() {
        m_Query = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>() },
                Any = EffectsComponentsToShow
            }
        );
    }

    protected override void OnUpdate() {
        var entities = m_Query.ToEntityArray(Allocator.TempJob);
        var targets = m_Query.ToComponentDataArray<GameplayEffectTargetComponent>(Allocator.TempJob);
        var durations = m_Query.ToComponentDataArray<GameplayEffectDurationComponent>(Allocator.TempJob);
        List<GameplayEffectDurationComponent> cooldowns = new List<GameplayEffectDurationComponent>();
        for (var i = 0; i < targets.Length; i++) {
            if (targets[i] == Player) {
                cooldowns.Add(durations[i].Value);
            }
        }

        entities.Dispose();
        targets.Dispose();
        durations.Dispose();

        // Sort list of cooldowns based on the nominal duration
        cooldowns.Sort((x, y) => x.Value.NominalDuration.CompareTo(y.Value.NominalDuration)*-1);

        // Set cooldowns
        for (var i = 0; i < cooldowns.Count; i++) {
            BuffIcons[i].CooldownOverlay.fillAmount = cooldowns[i].Value.RemainingTime / cooldowns[i].Value.NominalDuration;
        }
        // Reset all other images to 0
        for (var i = cooldowns.Count; i < BuffIcons.Count; i++) {
            BuffIcons[i].CooldownOverlay.fillAmount = 0;
        }
        // Update buffs bar with cooldowns
    }
}


