
using UnityEngine;

namespace AbilitySystem
{

    [CreateAssetMenu(menuName = "Gameplay Ability System/Turn Tick Provider")]
    public class TurnTickProvider : TickProvider
    {
        public bool TickTurn;

        [SerializeField] private int CurrentTurn;
        public override float TickMagnitude()
        {
            return TickTurn ? 1 : 0;
        }

        public int GetCurrentTurn()
        {
            return CurrentTurn;
        }

        public void SetTurn(int value)
        {
            CurrentTurn = value;
        }

        public void IncrementTurn()
        {
            CurrentTurn++;
        }



    }

}