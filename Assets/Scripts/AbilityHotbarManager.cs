/*
 * Created on Sat Nov 23 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using GameplayAbilitySystem.AbilitySystem.Components;
using GameplayAbilitySystem.Abilities.Components;
using MyGameplayAbilitySystem.Common.ScriptableObjects;

public class AbilityHotbarManager : MonoBehaviour {
    public ActorAbilitySystem AbilityCharacter;
    public List<AbilityHotbarButton> AbilityButtons;
    public AbilityIconMapScriptableObject AbilityIconMaps;

    public List<int> AbilityIdentifiers;

    void Start() {
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityOwnerEntity = AbilityCharacter.AbilityOwnerEntity;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityButtons = AbilityButtons;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityIdentifiers = AbilityIdentifiers;
        World.Active.GetOrCreateSystem<AbilityHotbarUpdateSystem>().AbilityIconMaps = AbilityIconMaps.AbilityIconMaps;
    }
}

public class AbilityHotbarUpdateSystem : ComponentSystem {
    public Entity AbilityOwnerEntity;

    [SerializeField]
    public List<AbilityHotbarButton> AbilityButtons;

    public List<int> AbilityIdentifiers;


    public List<AbilityIconMap> AbilityIconMaps;

    public Entity[] GrantedAbilityEntities = new Entity[8];

    protected override void OnUpdate() {
        // We need to store list of entities corresponding to the player's granted abilities
        // If the first ability is still null, then update the granted abilities list
        if (GrantedAbilityEntities[0] == default(Entity)) {
            UpdateGrantedAbilityEntitiesList();
        }
        
        for (int i = 0; i < AbilityButtons.Count; i++) {
            // reset all cooldowns
            AbilityButtons[i].SetCooldownRemainingPercent(1);
            var abilityIconMap = AbilityIconMaps.FirstOrDefault(x => x.AbilityIdentifier == AbilityIdentifiers[i]);
            if (abilityIconMap != null) {
                // Set icon to what is defined in Ability Icon Maps, if different
                AbilityButtons[i].ImageIcon.sprite = abilityIconMap.Sprite;
                AbilityButtons[i].ImageIcon.color = abilityIconMap.SpriteColor;
            } else {
                AbilityButtons[i].ImageIcon.sprite = null;
                AbilityButtons[i].ImageIcon.color = new Color32(0, 0, 0, 0);
            }


            if (World.Active.EntityManager.HasComponent<AbilityCooldownComponent>(GrantedAbilityEntities[i])) {
                var abilityCooldown = World.Active.EntityManager.GetComponentData<AbilityCooldownComponent>(GrantedAbilityEntities[i]);
                var abilityState = World.Active.EntityManager.GetComponentData<AbilityStateComponent>(GrantedAbilityEntities[i]);
                UpdateButton(i, abilityCooldown.Value.NominalDuration, abilityCooldown.Value.RemainingTime, abilityState > 0);
            }

        }

    }

    public void UpdateGrantedAbilityEntitiesList() {
        // Clear existing list of granted abilities
        for (var i = 0; i < GrantedAbilityEntities.Length; i++) {
            GrantedAbilityEntities[i] = default(Entity);
        }

        Entities
        .ForEach<AbilityOwnerComponent, AbilityCooldownComponent, AbilityStateComponent, AbilityIdentifierComponent>((Entity entity, ref AbilityOwnerComponent abilityOwner, ref AbilityCooldownComponent abilityCooldown, ref AbilityStateComponent state, ref AbilityIdentifierComponent identifier) => {
            // Only do this for the appropriate actor
            if (abilityOwner.Value == AbilityOwnerEntity) {
                // Check our list to see if this ability is defined
                var id = identifier.Value;
                var abilityIdentifierIndex = AbilityIdentifiers.FindIndex(x => x == id);
                if (abilityIdentifierIndex >= 0) {
                    GrantedAbilityEntities[abilityIdentifierIndex] = entity;
                }
            }

        });
    }


    private void UpdateButton(int index, float cooldownDuration, float cooldownTimeRemaining, bool cooldownActive) {

        var button = AbilityButtons[index];
        var remainingPercent = 1f;
        if (cooldownDuration != 0) {
            remainingPercent = 1 - cooldownTimeRemaining / cooldownDuration;
        }

        if (remainingPercent == 1 && cooldownActive) {
            remainingPercent = 0;
        }
        button.SetCooldownRemainingPercent(remainingPercent);
    }


}
