using GameplayAbilitySystem.Attributes;
using UnityEngine;

namespace AbilitySystemDemo
{

    public abstract class AbstractStatChangeHandler : ScriptableObject
    {
        public abstract void StatChanged(AttributeChangeData Change);
    }
}


