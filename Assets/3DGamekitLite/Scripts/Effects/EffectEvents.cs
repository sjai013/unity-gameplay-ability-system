using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit3D
{
    public class EffectEvents : MonoBehaviour
    {
        [System.Serializable]
        public class EventData
        {
            public string eventName;

            [System.Serializable]
            public class EventTarget
            {
                public GameObject gameObject;
                public bool state;

                [HideInInspector]
                public ParticleSystem particlesystem;
            }

            public EventTarget[] targets;
        }

        public EventData[] events;

        protected Dictionary<int, EventData> m_EventLookup = new Dictionary<int, EventData>();
        protected Animator m_Animator;

        void Start()
        {
            m_Animator = GetComponent<Animator>();
            if (m_Animator == null)
                throw new MissingComponentException("EffectEvents requires an animator!");

            SceneLinkedSMB<EffectEvents>.Initialise(m_Animator, this);

            m_EventLookup.Clear();
            for (int i = 0; i < events.Length; ++i)
            {
                m_EventLookup[Animator.StringToHash(events[i].eventName)] = events[i];

                //lookup if we have particle system on target as those need to be restarted on re-enabling, and faster to lookup only once on init
                for (int k = 0; k < events[i].targets.Length; ++k)
                {
                    events[i].targets[k].particlesystem =
                        events[i].targets[k].gameObject.GetComponentInChildren<ParticleSystem>();
                }
            }
        }

        public void PlayEvent(string eventName, bool inverted = false)
        {
            EventData data;
            if (!m_EventLookup.TryGetValue(Animator.StringToHash(eventName), out data))
            {
                Debug.LogErrorFormat(gameObject, "Couldn't find event {0} in this EffectEvents", eventName);
                return;
            }

            for (int i = 0; i < data.targets.Length; ++i)
            {
                bool state = data.targets[i].state;
                state = inverted ? !state : state;

                var target = data.targets[i];

                target.gameObject.SetActive(state);

                if (state && target.particlesystem != null)
                {//if we have a aprticle system and we re-enable the target, we need to restart the aprticle
                    target.particlesystem.time = 0;
                    target.particlesystem.Play(true);
                }
            }
        }
    }
}
