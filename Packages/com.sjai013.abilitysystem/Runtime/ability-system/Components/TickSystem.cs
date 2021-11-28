using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public abstract class TickSystem: MonoBehaviour
    {
        protected List<AbilitySystemCharacter> m_AbilitySystemCharacters = new List<AbilitySystemCharacter>(100);

        public void RegisterAbilitySystemCharacter(AbilitySystemCharacter asc)
        {
            // Check if the ASC already exists in this list.  If it doesn't exist, add it to list
            if (!m_AbilitySystemCharacters.Contains(asc))
            {
                m_AbilitySystemCharacters.Add(asc);
            }

        }
        public void UnregisterAbilitySystemCharacter(AbilitySystemCharacter asc)
        {
            m_AbilitySystemCharacters.Remove(asc);
        }

    }

}