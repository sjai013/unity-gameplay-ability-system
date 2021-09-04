using UnityEngine;

namespace MyGameplayAbilitySystem.SampleAbilities.Projectile
{
    [AddComponentMenu("Gameplay Ability System/Abilities/Projectile/Projectile Spawn")]
    public class ProjectileSpawn : MonoBehaviour
    {
        [SerializeField] private Transform m_Location;
        public Transform GetLocation()
        {
            return m_Location;
        }
    }
}