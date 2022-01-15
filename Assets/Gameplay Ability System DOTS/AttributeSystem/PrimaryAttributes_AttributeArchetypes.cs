using System.Linq;
namespace MyGameplayAbilitySystem.Attributes
{
    public static class PrimaryAttributeArchetypeFactory 
    {
        public static Unity.Entities.EntityArchetype PrimaryAttributeArchetype(Unity.Entities.EntityManager em)
        {
            return em.CreateArchetype((AttributeStrength.GetTypes()).Concat(AttributeIntelligence.GetTypes()).Concat(AttributeAgility.GetTypes()).ToArray());
        }
        public static Unity.Entities.EntityArchetype HeroAttributeArchetype(Unity.Entities.EntityManager em)
        {
            return em.CreateArchetype((AttributeStrength.GetTypes()).Concat(AttributeIntelligence.GetTypes()).Concat(AttributeAgility.GetTypes()).Concat(AttributeMaxHealth.GetTypes()).Concat(AttributeHealth.GetTypes()).Concat(AttributeMaxMana.GetTypes()).Concat(AttributeMana.GetTypes()).ToArray());
        }
    }
}