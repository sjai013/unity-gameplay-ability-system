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

