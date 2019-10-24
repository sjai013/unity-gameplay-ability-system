using NUnit.Framework;
using Unity.Entities;

namespace Tests {
    [Category("Attribute system Tests")]
    public class HealthAttributeSystemTests {
        [Test]
        public void TestSetup() {
            var entityManager = World.Active.EntityManager;
        }
    }
}