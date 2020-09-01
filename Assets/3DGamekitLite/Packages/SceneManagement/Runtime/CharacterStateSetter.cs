using System;
using System.Collections;
using UnityEngine;

namespace Gamekit3D
{
    /// <summary>
    /// This class is used to put the player character into a specific state, usually upon entering a scene.
    /// </summary>
    public class CharacterStateSetter : MonoBehaviour
    {
        [Serializable]
        public class ParameterSetter
        {
            public enum ParameterType
            {
                Bool, Float, Int, Trigger,
            }

            public string parameterName;
            public ParameterType parameterType;
            public bool boolValue;
            public float floatValue;
            public int intValue;

            protected int m_Hash;

            public void Awake()
            {
                m_Hash = Animator.StringToHash(parameterName);
            }

            public void SetParameter(Animator animator)
            {
                switch (parameterType)
                {
                    case ParameterType.Bool:
                        animator.SetBool(m_Hash, boolValue);
                        break;
                    case ParameterType.Float:
                        animator.SetFloat(m_Hash, floatValue);
                        break;
                    case ParameterType.Int:
                        animator.SetInteger(m_Hash, intValue);
                        break;
                    case ParameterType.Trigger:
                        animator.SetTrigger(m_Hash);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        public bool setCharacterVelocity;
        public Vector2 characterVelocity;

        public bool setCharacterFacing;
        public bool faceLeft;

        public Animator animator;

        public bool setState;
        public string animatorStateName;

        public bool setParameters;
        public ParameterSetter[] parameterSetters;

        int m_HashStateName;
        Coroutine m_SetCharacterStateCoroutine;

        void Awake()
        {
            m_HashStateName = Animator.StringToHash(animatorStateName);

            for (int i = 0; i < parameterSetters.Length; i++)
                parameterSetters[i].Awake();
        }

        public void SetCharacterState()
        {
            if (m_SetCharacterStateCoroutine != null)
                StopCoroutine(m_SetCharacterStateCoroutine);


            if (setState)
                animator.Play(m_HashStateName);

            if (setParameters)
            {
                for (int i = 0; i < parameterSetters.Length; i++)
                    parameterSetters[i].SetParameter(animator);
            }
        }

        public void SetCharacterState(float delay)
        {
            if (m_SetCharacterStateCoroutine != null)
                StopCoroutine(m_SetCharacterStateCoroutine);
            m_SetCharacterStateCoroutine = StartCoroutine(CallWithDelay(delay, SetCharacterState));
        }

        IEnumerator CallWithDelay(float delay, Action call)
        {
            yield return new WaitForSeconds(delay);
            call();
        }
    }
}