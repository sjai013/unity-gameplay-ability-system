using UnityEngine;
namespace AbilitySystem
{
    public abstract class TickProvider : ScriptableObject
    {
        public abstract float TickMagnitude();

    }
}