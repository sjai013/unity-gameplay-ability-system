using GameplayAbilitySystem.Attributes.Components;
using UnityEngine;

namespace AbilitySystemDemo
{

    public abstract class AbstractStatChangeHandler : ScriptableObject
    {
        public abstract void StatChanged(AttributeChangeData Change);
    }
}


