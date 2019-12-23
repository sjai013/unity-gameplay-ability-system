/*
 * Created on Mon Dec 23 2019
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

using System.Collections;
using System.Collections.Generic;
using GameplayAbilitySystem.Attributes.ScriptableObjects;
using MyGameplayAbilitySystem.AbilitySystem.MonoBehaviours;
using MyGameplayAbilitySystem.Common.ScriptableObjects;
using Unity.Entities;
using UnityEngine;

public class BuffBarManager : MonoBehaviour {
    public BuffsContainerScriptableObject EffectsToShow;
    public List<GameplayTagStatusBarButton> BuffUIObject;
    public BuffIconMapScriptableObject BuffIconMaps;
    public ActorAbilitySystem AbilitySystem;
    public HashSet<ComponentType> ComponentTypes { get; private set; }
    public HashSet<int> Buffs { get; private set; }
    private EntityQuery Query;

    public Dictionary<int, BuffIconMap> BuffIconForIdentifier { get; private set; }

    // Start is called before the first frame update
    void Start() {
        this.ComponentTypes = new HashSet<ComponentType>(EffectsToShow.ComponentTypes);
        this.Buffs = new HashSet<int>(EffectsToShow.GetIndices());
        BuffIconForIdentifier = new Dictionary<int, BuffIconMap>();
        for (var i = 0; i < BuffIconMaps.BuffIconMaps.Count; i++) {
            var iconMap = BuffIconMaps.BuffIconMaps[i];
            BuffIconForIdentifier[iconMap.BuffIdentifier] = iconMap;
        }
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuffBarUpdateSystem>().RegisterBuffBar(this);
    }

    // Update is called once per frame
    void Update() {

    }
}


