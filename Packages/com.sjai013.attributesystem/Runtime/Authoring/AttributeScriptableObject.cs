using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AttributeSystem.Authoring
{
    /// <summary>
    /// This asset defines a single player attribute
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Attribute")]
    public class AttributeScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Friendly name of this attribute.  Used for dislpay purposes only.
        /// </summary>
        public string Name;
    }
}
