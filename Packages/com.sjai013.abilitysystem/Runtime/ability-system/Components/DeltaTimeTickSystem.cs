using UnityEngine;

namespace AbilitySystem
{
    public class DeltaTimeTickSystem : TickSystem
    {
        private void Tick(float tickValue)
        {
            for (var i = 0; i < m_AbilitySystemCharacters.Count; i++)
            {
                m_AbilitySystemCharacters[i].Tick(tickValue);
            }
        }

        void Update()
        {
            Tick(Time.deltaTime);
        }
    }

}