
using UnityEngine;

namespace AbilitySystem
{

    public class TurnController : MonoBehaviour
    {
        [SerializeField] private int m_Turn = 0;
        [SerializeField] private bool m_NextTurn;
        [SerializeField] public bool TickTurn { get; private set; }
        public void Update()
        {
            if (m_NextTurn)
            {
                m_Turn += 1;
                TickTurn = true;
                m_NextTurn = false;
            }
            else
            {
                TickTurn = false;
            }
        }

        public int GetCurrentTurn()
        {
            return m_Turn;
        }

    }

}