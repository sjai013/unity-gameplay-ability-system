using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Gamekit3D.GameCommands
{

    public class Platform : MonoBehaviour
    {
        public LayerMask layers;

        protected CharacterController m_CharacterController;

        const float k_SqrMaxCharacterMovement = 1f;

        void OnTriggerStay(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                CharacterController character = other.GetComponent<CharacterController>();

                if (character != null)
                    m_CharacterController = character;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (0 != (layers.value & 1 << other.gameObject.layer))
            {
                if (m_CharacterController != null && other.gameObject == m_CharacterController.gameObject)
                    m_CharacterController = null;
            }
        }

        public void MoveCharacterController(Vector3 deltaPosition)
        {
            if (m_CharacterController != null && deltaPosition.sqrMagnitude < k_SqrMaxCharacterMovement)
            {
                m_CharacterController.Move(deltaPosition);
            }
        }
    }
}