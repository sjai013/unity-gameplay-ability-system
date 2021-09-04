using System;
using System.Collections.Generic;
using Cinemachine;
using GameplayAbilitySystemDemo.Input;
using GameplayTag.Authoring;
using UnityEngine;
using UnityEngine.VFX;

namespace GameplayAbilitySystemDemo
{
    public class PlayerMovement : MonoBehaviour
    {
        [Flags]
        public enum PlayerStates
        {
            Idle = 0,
            Moving = 1,
            Jumping = 2
        }

        [SerializeField] private float m_GroundTolerance = 0.1f;
        [SerializeField] private float m_MovementSpeed = 1f;
        [SerializeField] private float m_JumpForce = 5f;
        [SerializeField] private float m_SlamForce = 10f;
        [SerializeField] private bool m_IsGrounded;
        [SerializeField] private PlayerStates m_PlayerState;
        private PlayerStates m_PreviousPlayerState;
        [SerializeField] private Transform m_FeetLocation;
        [SerializeField] GameObject m_JumpVfx;

        DefaultInputActions m_InputActions;
        Rigidbody2D m_Rb;
        BoxCollider2D m_Col;
        private Vector2 m_MovementVector;
        private int groundedMask;
        private AimTarget m_AimTarget;
        private bool m_DoubleJumped;
        private bool m_JumpVfxTrigger;

        private Vector3 m_JumpPosition;

        void Start()
        {
            m_InputActions = new DefaultInputActions();
            m_InputActions.PlayerMovement.Enable();
            m_Rb = GetComponent<Rigidbody2D>();
            m_Col = GetComponent<BoxCollider2D>();

            groundedMask = LayerMask.GetMask("Ground");
            m_AimTarget = GetComponent<AimTarget>();

        }

        void Update()
        {
            this.m_PreviousPlayerState = this.m_PlayerState;
            m_MovementVector = m_InputActions.PlayerMovement.Move.ReadValue<Vector2>().normalized;
            m_IsGrounded = IsGrounded();


            if (m_MovementVector.magnitude < 0.2f) m_MovementVector = Vector2.zero;
            if (!m_IsGrounded) m_MovementVector = m_MovementVector / 1.2f;

            m_MovementVector.y = 0;

            SetAimTargetFlip();
            HandleJump();
            UpdatePlayerState();
            HandleVFX();
        }

        void SetAimTargetFlip()
        {
            if (m_MovementVector.x < 0)
            {
                m_AimTarget.SetFlip(true);
            }

            if (m_MovementVector.x > 0)
            {
                m_AimTarget.SetFlip(false);
            }

        }

        void HandleVFX()
        {

            if (m_JumpVfxTrigger)
            {
                var go = Instantiate(m_JumpVfx, m_FeetLocation.position, Quaternion.identity);
                Destroy(go, 2f);
                m_JumpVfxTrigger = false;
            }
        }

        void UpdatePlayerState()
        {
            this.m_PlayerState = PlayerStates.Idle;


            if (!m_IsGrounded)
            {
                this.m_PlayerState |= PlayerStates.Jumping;
            }

            if (m_MovementVector.magnitude > 0)
            {
                this.m_PlayerState |= PlayerStates.Moving;
            }


        }

        public PlayerStates GetState()
        {
            return this.m_PlayerState;
        }

        bool HandleJump()
        {
            var jumpTriggered = m_InputActions.PlayerMovement.Jump.triggered;
            var slamTriggered = m_InputActions.PlayerMovement.Slam.triggered;

            if (jumpTriggered)
            {
                if (m_IsGrounded)
                {
                    m_Rb.AddForce(transform.up * m_JumpForce, ForceMode2D.Impulse);
                    m_JumpVfxTrigger = true;
                    return true;
                }
                else
                {
                    // Double Jump
                    if (!m_DoubleJumped)
                    {
                        // If y velocity isn't set to 0, the add force impulses will combine, causing very high jumps
                        m_Rb.velocity = new Vector2(m_Rb.velocity.x, 0);
                        m_Rb.AddForce(transform.up * m_JumpForce, ForceMode2D.Impulse);
                        m_DoubleJumped = true;
                        m_JumpVfxTrigger = true;
                    }
                }
            }

            if (slamTriggered && !jumpTriggered && !m_IsGrounded)
            {
                //Slam down
                m_Rb.AddForce(-transform.up * m_SlamForce, ForceMode2D.Impulse);
            }

            if (m_IsGrounded)
            {
                m_DoubleJumped = false;
            }
            return false;
        }
        bool IsGrounded()
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(m_Col.bounds.center, -transform.up, m_Col.bounds.extents.y + m_GroundTolerance, groundedMask);
            Debug.DrawRay(m_Col.bounds.center, Vector2.down * (m_Col.bounds.extents.y + m_GroundTolerance), raycastHit.collider == null ? Color.red : Color.green);
            return raycastHit.collider != null;
        }

        void FixedUpdate()
        {
            var movementVelocity = m_MovementVector * m_MovementSpeed;
            m_Rb.velocity = new Vector2(movementVelocity.x, m_Rb.velocity.y);
        }
    }

}
