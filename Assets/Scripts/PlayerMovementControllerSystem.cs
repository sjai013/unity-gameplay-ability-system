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

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerControllerSystem : JobComponentSystem {
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [BurstCompile]
    [RequireComponentTag(typeof(PlayerMovementControllableTagComponent))]
    struct PlayerControllerSystemJob : IJobForEach<Translation, Rotation, PlayerMovementRotationMultiplierComponent, PlayerMovementSpeedMultiplierComponent> {
        // Add fields here that your job needs to do its work.
        // For example,
        //    public float deltaTime;

        public float2 movementVector;
        public float deltaTime;

        public void Execute(ref Translation translation, ref Rotation rotation, [ReadOnly] ref PlayerMovementRotationMultiplierComponent rotationMultiplierComponent, [ReadOnly] ref PlayerMovementSpeedMultiplierComponent speedMultiplierComponent) {
            translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * movementVector.y * deltaTime * speedMultiplierComponent.Value;
            
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), movementVector.x * rotationMultiplierComponent.Value * deltaTime);
            rotation.Value = math.mul(q, rotation.Value);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies) {
        float2 movementVector;
        movementVector.x = Input.GetAxis("Horizontal");
        movementVector.y = Input.GetAxis("Vertical");
        var job = new PlayerControllerSystemJob
        {
            deltaTime = Time.deltaTime,
            movementVector = movementVector

        };

        // Assign values to the fields on your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
        //     job.deltaTime = UnityEngine.Time.deltaTime;



        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}