
using UnityEngine;

namespace AbilitySystem
{

    [CreateAssetMenu(menuName = "Gameplay Ability System/Time Tick Provider")]
    public class TimeTickProvider : TickProvider
    {
        [SerializeField] private float m_Scale = 1;
        public override float TickMagnitude()
        {
            return Time.deltaTime;
        }
    }

}