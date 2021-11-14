
using UnityEngine;

namespace AbilitySystem
{

    public class TurnController : MonoBehaviour
    {

        [SerializeField] private bool m_NextTurn;
        [SerializeField] private TurnTickProvider m_TurnTickProvider;
        public void Update()
        {
            if (m_NextTurn)
            {
                m_TurnTickProvider.IncrementTurn();
                m_TurnTickProvider.TickTurn = true;
                m_NextTurn = false;
            }
            else
            {
                m_TurnTickProvider.TickTurn = false;
            }
        }

        public void NextTurn()
        {
            m_NextTurn = true;
        }

        public int GetCurrentTurn()
        {
            return m_TurnTickProvider.GetCurrentTurn();
        }

    }

}