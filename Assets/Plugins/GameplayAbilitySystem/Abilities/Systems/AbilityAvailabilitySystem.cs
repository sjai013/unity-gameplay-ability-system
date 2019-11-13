/*
 * Created on Mon Nov 04 2019
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

using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Abilities.Systems {
    /// <summary>
    /// Updates the Ability State to indicate if the ability is available.
    /// 
    /// The Ability State can be checked before executing ability to make sure
    /// the actor has been granted the ability, and it is currently available to use
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbilityAvailabilitySystem<T> : JobComponentSystem
    where T : struct, IAbilityTagComponent, IComponentData {
        protected abstract JobHandle UpdateAbilityAvailability(JobHandle inputDeps);
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            inputDeps = UpdateAbilityAvailability(inputDeps);
            return inputDeps;
        }
    }
}


// /// <summary>
// /// The ability owns these effects.
// /// </summary>
// /// <value></value>
// protected virtual ComponentType[] AbilityOwningEffects { get; }



// /// <summary>
// /// This ability cancels currently *executing* abilities which own these effects.
// /// E.g. cancelling a chanelling spell
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] CancelAbilitiesWithOwningEffects { get; }

// /// <summary>
// /// Prevents execution of abilities that have these effects
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] BlockAbilitiesWithEffects { get; }

// /// <summary>
// /// Provides the actor executing this ability with these effects. 
// /// These effects are automatically removed once the ability has
// /// finished executing.
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] SourceActivationOwnedEffects { get; }

// /// <summary>
// /// The actor needs to have all these tags applied to begin executing
// /// the ability.  
// /// 
// /// E.g. Allowing casting of Fire 2 only if we have already cast Fire 1
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] SourceActivationRequiredTags { get; }

// /// <summary>
// /// The actor must not have any of these tags to begin executing the ability.
// /// 
// /// E.g. Can't cast magic if the actor is silenced.
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] SourceActivationBlockedTags { get; }

// /// <summary>
// /// The target needs to have all these tags applied to begin executing
// /// the ability.  
// /// 
// /// E.g. Allowing casting Remedy on a silenced target, only if the target is poisoned.
// /// 
// /// For AOE effects, this means that the spell will still cast, but will have no effect on
// /// targets that do not have all these tags.
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] TargetRequiredTags { get; }

// /// <summary>
// /// The target must not have any of these tags to begin executing the ability.
// /// 
// /// E.g. Can't cast magic if the actor is silenced.
// /// 
// /// For AOE effects, this means that the spell will still cast, but will have no effect
// /// on targets that have any of these tags.
// /// </summary>
// /// <value></value>
// protected abstract ComponentType[] TargetBlockedTags { get; }

