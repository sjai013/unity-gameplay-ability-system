using System;
using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.ScriptableObjects;
using GameplayAbilitySystem.GameplayEffects.Components;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class BuffBarManager : MonoBehaviour {
    public BuffsContainerScriptableObject EffectsToShow;
    public List<GameplayTagStatusBarButton> BuffIcons;
    public ActorAbilitySystem AbilitySystem;
    public HashSet<ComponentType> ComponentTypes { get; private set; }
    public HashSet<int> Buffs { get; private set; }
    private EntityQuery Query;

    // Start is called before the first frame update
    void Start() {
        this.ComponentTypes = new HashSet<ComponentType>(EffectsToShow.ComponentTypes);
        this.Buffs = new HashSet<int>(EffectsToShow.GetIndices());
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuffBarUpdateSystem>().RegisterBuffBar(this);
    }

    // Update is called once per frame
    void Update() {

    }
}

public class BuffBarUpdateSystem : JobComponentSystem {
    EntityQuery m_Query;
    private Dictionary<Entity, List<BuffBarManager>> BuffBars = new Dictionary<Entity, List<BuffBarManager>>();
    public void RegisterBuffBar(BuffBarManager buffBarManager) {
        // Add the manager to container
        if (!BuffBars.TryGetValue(buffBarManager.AbilitySystem.AbilityOwnerEntity, out var buffBarsForPlayer)) {
            buffBarsForPlayer = new List<BuffBarManager>();
            BuffBars.Add(buffBarManager.AbilitySystem.AbilityOwnerEntity, buffBarsForPlayer);
        }
        buffBarsForPlayer.Add(buffBarManager);
    }

    public void UnregisterBuffBar(BuffBarManager buffBarManager) {
        // Remove the manager from container
        if (BuffBars.TryGetValue(buffBarManager.AbilitySystem.AbilityOwnerEntity, out var buffBars)) {
            buffBars.Remove(buffBarManager);
        }
    }

    protected override void OnCreate() {
        m_Query = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new ComponentType[] { ComponentType.ReadOnly<GameplayEffectDurationComponent>(), ComponentType.ReadOnly<GameplayEffectTargetComponent>(), ComponentType.ReadOnly<GameplayEffectBuffIndex>() },
            }
        );
    }
    struct EffectBuffTuple : IComparable<EffectBuffTuple> {
        public GameplayEffectDurationComponent DurationComponent;
        public GameplayEffectBuffIndex BuffComponent;
        public int CompareTo(EffectBuffTuple other) {
            return DurationComponent.Value.NominalDuration < other.DurationComponent.Value.NominalDuration ? 1 : -1;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        NativeMultiHashMap<Entity, EffectBuffTuple> buffDurations = new NativeMultiHashMap<Entity, EffectBuffTuple>(m_Query.CalculateEntityCount(), Allocator.TempJob);
        var buffDurationsConcurrent = buffDurations.AsParallelWriter();
        // inputDeps.Complete();
        inputDeps = Entities
            .ForEach((in GameplayEffectDurationComponent duration, in GameplayEffectTargetComponent target, in GameplayEffectBuffIndex buffIndex) => {
                buffDurationsConcurrent.Add(target, new EffectBuffTuple { BuffComponent = buffIndex, DurationComponent = duration });
            })
            .WithStoreEntityQueryInField(ref m_Query)
            .Schedule(inputDeps);

        inputDeps.Complete();
        // For every player that we need to worry about
        foreach (var buffBarManager in BuffBars) {
            NativeList<EffectBuffTuple> buffDurationTuple = new NativeList<EffectBuffTuple>(Allocator.Temp);
            // Get all items in the NMHM that have the same key
            if (buffDurations.TryGetFirstValue(buffBarManager.Key, out var effectBuffTuple, out var it)) {
                buffDurationTuple.Add(effectBuffTuple);

                while (buffDurations.TryGetNextValue(out effectBuffTuple, ref it)) {
                    buffDurationTuple.Add(effectBuffTuple);
                }
            }
            buffDurationTuple.Sort<EffectBuffTuple>();
            for (var iManager = 0; iManager < buffBarManager.Value.Count; iManager++) {
                for (var i = 0; i < buffDurationTuple.Length; i++) {
                    buffBarManager.Value[iManager].BuffIcons[i].CooldownOverlay.fillAmount = buffDurationTuple[i].DurationComponent.Value.RemainingTime / buffDurationTuple[i].DurationComponent.Value.NominalDuration;
                }
                // Reset all other images to 0
                for (var i = buffDurationTuple.Length; i < buffBarManager.Value[iManager].BuffIcons.Count; i++) {
                    buffBarManager.Value[iManager].BuffIcons[i].CooldownOverlay.fillAmount = 0;
                }
            }

            buffDurationTuple.Dispose();
        }

        // Set cooldowns

        buffDurations.Dispose();

        return inputDeps;
    }
}


