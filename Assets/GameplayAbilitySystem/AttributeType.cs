using System.Collections.Generic;
using UnityEngine;

namespace GameplayAbilitySystem.Attributes.Components {
    [CreateAssetMenu(fileName = "Attribute Type", menuName = "Ability System/Attribute Type")]
    public class AttributeType : ScriptableObject {
        public static Dictionary<int, AttributeType> Attributes = new Dictionary<int, AttributeType>();
        [SerializeField]
        public int AttributeId;

        public void OnEnable() {
            Attributes[AttributeId] = this;
        }
    }
}
