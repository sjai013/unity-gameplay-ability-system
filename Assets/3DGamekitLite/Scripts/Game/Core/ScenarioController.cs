using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D
{
    public class ScenarioController : MonoBehaviour
    {
        [System.Serializable]
        public class ScenarioObjective
        {
            public string name;
            public int requiredCount;
            [HideInInspector] public bool completed = false;
            [HideInInspector] public int currentCount;
            [HideInInspector] public bool eventFired = false;
            public UnityEvent OnComplete, OnProgress;

            public void Complete()
            {
                currentCount += 1;
                if (currentCount >= requiredCount)
                {
                    completed = true;
                    if (!eventFired) OnComplete.Invoke();
                    eventFired = true;
                }
                else
                {
                    OnProgress.Invoke();
                }
            }
        }

        public UnityEvent OnAllObjectivesComplete;

        [SerializeField]
        List<ScenarioObjective> objectives = new List<ScenarioObjective>();


        public bool AddObjective(string name, int requiredCount)
        {
            for (var i = 0; i < objectives.Count; i++)
            {
                if (objectives[i].name == name) return false;
            }
            objectives.Add(new ScenarioObjective() { name = name, requiredCount = requiredCount });
            objectives.Sort((A, B) => A.name.CompareTo(B.name));
            return true;
        }

        public void RemoveObjective(string name)
        {
            for (var i = 0; i < objectives.Count; i++)
            {
                if (objectives[i].name == name)
                {
                    objectives.RemoveAt(i);
                    return;
                }
            }
        }

        public ScenarioObjective[] GetAllObjectives()
        {
            return objectives.ToArray();
        }

        public void CompleteObjective(string name)
        {
            for (var i = 0; i < objectives.Count; i++)
            {
                if (objectives[i].name == name)
                {
                    objectives[i].Complete();
                }
            }
            for (var i = 0; i < objectives.Count; i++)
            {
                if (!objectives[i].completed) return;
            }
            OnAllObjectivesComplete.Invoke();
        }


    }

}