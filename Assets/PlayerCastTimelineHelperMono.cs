/*
 * Created on Tue Dec 03 2019
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
using MyGameplayAbilitySystem.Abilities;
using MyGameplayAbilitySystem.Abilities.DefaultAttack;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCastTimelineHelperMono : MonoBehaviour {
    // Start is called before the first frame update
    public bool isSwinging { get; set; }
    public PlayableAsset SwingWeaponPlayable;
    private PlayableDirector playableDirector;
    void Start() {
        playableDirector = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void PlaySwingAnimation() {
        playableDirector.Play(SwingWeaponPlayable);
    }

    public IEnumerator CheckForSwingHit(bool hit, EntityManager EntityManager, Entity targetEntity) {
        // Wait for swing to begin
        while (!isSwinging) yield return null;

        // Swing started, check if we've hit something, or if the swing check period is complete
        while (isSwinging) {
            yield return null;
        }

        if (hit) {
            (new DefaultAttackAbilityTag()).CreateTargetAttributeModifiers(EntityManager, targetEntity);
        }
    }
}
