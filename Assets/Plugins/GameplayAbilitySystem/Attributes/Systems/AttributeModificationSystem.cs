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

using GameplayAbilitySystem.Attributes.Components;
using Unity.Entities;
using Unity.Jobs;

namespace GameplayAbilitySystem.Attributes.Systems {

    /// <summary>
    /// This is the base for all attribute modification systems.  
    /// Custom attribute modification types should inherit from this class
    /// and modify as necessary.
    /// 
    /// See <see cref="GenericAttributeSystem{TAttributeTag}"> for a sample modifier system
    /// </summary>
    /// <typeparam name="TAttribute">The attribute this system modifies</typeparam>
    public abstract class AttributeModificationSystem<TAttribute> : JobComponentSystem
    where TAttribute : struct, IAttributeComponent, IComponentData {

        /// <summary>
        /// This is the list of queries that are use
        /// </summary>
        protected EntityQuery[] Queries = new EntityQuery[3];
        protected EntityQuery actorsWithAttributesQuery;
        protected EntityQuery CreateQuery<TOper>()
        where TOper : struct, IAttributeOperator, IComponentData {
            return GetEntityQuery(
                ComponentType.ReadOnly<AttributeComponentTag<TAttribute>>(),
                ComponentType.ReadOnly<TOper>(),
                ComponentType.ReadOnly<AttributeModifier<TOper, TAttribute>>(),
                ComponentType.ReadOnly<AttributesOwnerComponent>()
                );
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            inputDependencies = ScheduleJobs(inputDependencies);
            return inputDependencies;
        }
        protected abstract JobHandle ScheduleJobs(JobHandle inputDependencies);
    }

}


