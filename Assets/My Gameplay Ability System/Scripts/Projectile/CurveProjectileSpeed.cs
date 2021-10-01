using UnityEngine;

namespace MyGameplayAbilitySystem.SampleAbilities.Projectile
{
    public class CurveProjectileSpeed : MonoBehaviour, IProjectileSpeedEvaluator
    {
        [SerializeField] private float m_SpeedMultiplier;
        [SerializeField] private AnimationCurve m_AnimationCurve;
        public float Evaluate(float t)
        {
            return m_AnimationCurve.Evaluate(t) * m_SpeedMultiplier;
        }
    }
}