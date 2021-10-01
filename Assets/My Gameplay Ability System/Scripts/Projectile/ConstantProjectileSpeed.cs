using UnityEngine;

namespace MyGameplayAbilitySystem.SampleAbilities.Projectile
{
    public class ConstantProjectileSpeed : MonoBehaviour, IProjectileSpeedEvaluator
    {
        [SerializeField] private float m_Speed;
        public float Evaluate(float t)
        {
            return m_Speed;
        }
    }
}