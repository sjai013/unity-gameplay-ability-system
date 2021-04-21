using System.Collections.Generic;
using AttributeSystem.Authoring;
using UnityEngine;

namespace AttributeSystem.Components
{
    public abstract class AbstractAttributeEventHandler : ScriptableObject
    {
        public abstract void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues);
    }
}
