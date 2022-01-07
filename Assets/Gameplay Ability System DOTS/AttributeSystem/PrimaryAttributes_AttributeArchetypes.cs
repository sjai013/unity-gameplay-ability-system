namespace MyGameplayAbilitySystem.Attributes
{
    public static class PrimaryAttributeArchetypeFactory 
    {
        public static Unity.Entities.EntityArchetype PrimaryAttributeArchetype(Unity.Entities.EntityManager em)
        {
            return em.CreateArchetype(AttributeStrength.GetTypes(),AttributeIntelligence.GetTypes(),AttributeAgility.GetTypes());
        }
        public static Unity.Entities.EntityArchetype HeroAttributeArchetype(Unity.Entities.EntityManager em)
        {
            return em.CreateArchetype(AttributeStrength.GetTypes(),AttributeIntelligence.GetTypes(),AttributeAgility.GetTypes(),AttributeMaxHealth.GetTypes(),AttributeHealth.GetTypes(),AttributeMaxMana.GetTypes(),AttributeMana.GetTypes());
        }
    }
}