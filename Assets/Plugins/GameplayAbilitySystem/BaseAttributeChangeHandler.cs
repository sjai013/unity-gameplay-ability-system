using GameplayAbilitySystem.Interfaces;
using UnityEngine;


namespace GameplayAbilitySystem.Attributes {
    public class BaseAttributeChangeHandler : ScriptableObject {

        public virtual void OnAttributeChange(IAttribute Attribute, ref float Value) {

        }
    }
}
