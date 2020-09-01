using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class EffectStateMachineBehavior : SceneLinkedSMB<EffectEvents>
    {
        public enum EventType
        {
            ENTER,
            EXIT
        }

        [System.Serializable]
        public class EventInstance
        {
            public string eventName;
            public bool invert;
            public EventType eventType;
        }

        public EventInstance[] events;

        protected List<EventInstance> m_EnterEvents = new List<EventInstance>();
        protected List<EventInstance> m_ExitEvents = new List<EventInstance>();

        private void OnEnable()
        {
            m_EnterEvents.Clear();
            m_ExitEvents.Clear();

            for (int i = 0; i < events.Length; ++i)
            {
                if (events[i].eventType == EventType.ENTER)
                {
                    m_EnterEvents.Add(events[i]);
                }
                else
                {
                    m_ExitEvents.Add(events[i]);
                }
            }
        }

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            for (int i = 0; i < m_EnterEvents.Count; ++i)
            {
                m_MonoBehaviour.PlayEvent(m_EnterEvents[i].eventName, m_EnterEvents[i].invert);
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            for (int i = 0; i < m_ExitEvents.Count; ++i)
            {
                m_MonoBehaviour.PlayEvent(m_ExitEvents[i].eventName, m_ExitEvents[i].invert);
            }
        }
    }
}