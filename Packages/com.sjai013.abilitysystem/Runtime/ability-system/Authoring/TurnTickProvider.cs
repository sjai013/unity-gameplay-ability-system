
using UnityEngine;

namespace AbilitySystem
{

    [CreateAssetMenu(menuName = "Gameplay Ability System/Turn Tick Provider")]
    public class TurnTickProvider : TickProvider
    {
        [SerializeField] TurnController m_TurnController;
        public override float TickMagnitude()
        {
            return m_TurnController.TickTurn ? 1 : 0;
        }

    }

}