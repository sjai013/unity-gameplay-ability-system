using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamekit3D.GameCommands
{
    //Base class to send command on different events (see in SendOnTrigger, SendOnBecameVisible etc. for example of subclasses)
    [SelectionBase]
    public class SendGameCommand : MonoBehaviour
    {
        //The type of Command to send. This is not link to any UnityEvent and just act as a way to differentiate this command from other in GameCommandHandlers
        [Tooltip("The type of command to send.")]
        public GameCommandType interactionType;
        [Tooltip("This object will receive the command.")]
        public GameCommandReceiver interactiveObject;
        [Tooltip("If set to true, this command will only be sent once.")]
        public bool oneShot = false;
        [Tooltip("How many seconds must pass before the command is sent again.")]
        public float coolDown = 1;
        [Tooltip("If not null, this audio source will be played when the command is sent.")]
        public AudioSource onSendAudio;
        [Tooltip("If onSendAudio is not null, it will play after this time has passed.")]
        public float audioDelay;

        float lastSendTime;
        bool isTriggered = false;

        public float Temperature
        {
            get
            {
                return 1f - Mathf.Clamp01(Time.time - lastSendTime);
            }
        }

        [ContextMenu("Send Interaction")]
        public void Send()
        {
            if (oneShot && isTriggered) return;
            if (Time.time - lastSendTime < coolDown) return;
            isTriggered = true;
            lastSendTime = Time.time;
            interactiveObject.Receive(interactionType);
            if (onSendAudio) onSendAudio.PlayDelayed(audioDelay);
        }

        protected virtual void Reset()
        {
            interactiveObject = GetComponent<GameCommandReceiver>();
        }
    }

}
