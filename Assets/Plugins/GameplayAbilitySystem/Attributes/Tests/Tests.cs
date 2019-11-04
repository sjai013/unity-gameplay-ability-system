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

using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;
using GameplayAbilitySystem.Attributes.Components;

namespace GameplayAbilitySystem.Attributes.Tests {
    [TestFixture]
    public class Tests {

        protected World world;
        protected EntityManager entityManager;

        [Test]
        public void TryCreateEntitiesAndMakeSureThatWorks() {
            var entities = new List<Entity>();
            var numEntities = 1000;
            // Create 1000 entities for a Health attribute
            for (var i = 0; i < numEntities; i++) {
                var entity = entityManager.CreateEntity();
                entities.Add(entity);
            }

            Assert.AreEqual(numEntities, entities.Count);
        }

        [Test]
        public void CreateAttributesWithHealthAndMakeSureNumberOfEntitiesIsCorrect() {
            var entities = new List<Entity>();
            var archetype = entityManager.CreateArchetype(typeof(HealthAttributeComponent));
            var numEntities = 1000;
            // Create 1000 entities for a Health attribute
            for (var i = 0; i < numEntities; i++) {
                var entity = entityManager.CreateEntity(archetype);
                entities.Add(entity);
            }

            Assert.AreEqual(numEntities, entities.Count);
        }




        [SetUp]
        public void Setup() {
            this.world = new World("Test World");
            entityManager = this.world.EntityManager;
        }

        [TearDown]
        public void Teardown() {
            foreach (var system in world.Systems) {
                world.DestroySystem(system);
            }
            world.Dispose();
            world = null;
        }

    }

}

