public static class PrimaryAttributeGroup {
        public static Unity.Entities.EntityArchetype PrimaryArchetype(Unity.Entities.EntityManager em)
    {
        return em.CreateArchetype(AttributeStrength.GetTypes(),AttributeIntelligence.GetTypes(),AttributeAgility.GetTypes());
    }
}
public static class HeroAttributeGroup {
        public static Unity.Entities.EntityArchetype HeroArchetype(Unity.Entities.EntityManager em)
    {
        return em.CreateArchetype(AttributeStrength.GetTypes(),AttributeIntelligence.GetTypes(),AttributeAgility.GetTypes(),AttributeMaxHealth.GetTypes(),AttributeHealth.GetTypes(),AttributeMaxMana.GetTypes(),AttributeMana.GetTypes());
    }
}