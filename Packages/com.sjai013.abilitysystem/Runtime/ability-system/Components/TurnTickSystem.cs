using UnityEngine;

namespace AbilitySystem
{
    public class TurnTickSystem : TickSystem
    {
        public void NextTurn()
        {
            for (var i = 0; i < m_AbilitySystemCharacters.Count; i++)
            {
                m_AbilitySystemCharacters[i].Tick(1f);
            }
        }

    }

}