using UnityEngine.Events;

namespace Gamekit3D.GameCommands
{

    public class TriggerUnityEvent : GameCommandHandler
    {
        public UnityEvent unityEvent;

        public override void PerformInteraction()
        {
            unityEvent.Invoke();
        }
    }
}
