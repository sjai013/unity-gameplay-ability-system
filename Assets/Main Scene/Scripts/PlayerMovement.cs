using System;
using AbilitySystem;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayAbilitySystemDemo.Input;
using MyGameplayAbilitySystem.StateMachine;
using Unity.Mathematics;
using UnityEngine;

namespace GameplayAbilitySystemDemo
{
    public class PlayerMovement : MonoBehaviour
    {
        const int JUMP_GROUNDED = 0;
        const int JUMP_FALLING = 10;
        const int JUMP_JUMPING = 20;
        const int JUMP_DOUBLEJUMP = 30;
        const int JUMP_SLAM = 40;

        const int MOVE_IDLE = 0;
        const int MOVE_NORMAL = 10;
        const int MOVE_DASH = 20;

        [Flags]
        public enum PlayerStates
        {
            Idle = 0,
            Moving = 1,
            Jumping = 2
        }

        [SerializeField] private AbstractAbility m_DashAbility;
        [SerializeField] private AttributeScriptableObject m_SpeedAttribute;
        [SerializeField] private float m_GroundTolerance = 0.1f;
        [SerializeField] private float m_JumpForce = 5f;
        [SerializeField] private float m_SlamForce = 10f;
        [SerializeField] private float m_DashDistance = 1.4f;
        [SerializeField] private bool m_IsGrounded;
        [SerializeField] private PlayerStates m_PlayerState;
        private PlayerStates m_PreviousPlayerState;
        [SerializeField] private Transform m_FeetLocation;
        [SerializeField] GameObject m_JumpVfx;

        DefaultInputActions m_InputActions;
        Rigidbody2D m_Rb;
        BoxCollider2D m_Col;
        private Vector2 m_MovementVector;
        private AttributeSystemComponent m_AttributeSystem;
        private AbilitySystemCharacter m_AbilitySystemCharacter;
        [SerializeField] private float m_MovementSpeed = 1f;
        private int groundedMask;
        private AimTarget m_AimTarget;
        private bool m_DoubleJumped;
        private bool m_JumpVfxTrigger;

        private Vector3 m_JumpPosition;

        StateMachine MoveStateMachine;
        StateMachine JumpStateMachine;
        float groundedInhibitTime = 0;

        float m_DashTime = 0f;

        void InitialiseMovementStateMachine()
        {
            State IdleState = new State(MOVE_IDLE);
            State MoveState = new State(MOVE_NORMAL);

            IdleState.Enter = () =>
            {
                m_Rb.velocity = new Vector2(0, m_Rb.velocity.y);
            };

            IdleState.Active = (StateMachine stateMachine) =>
            {
                if (m_MovementVector != Vector2.zero)
                {
                    stateMachine.NextState(MoveState);
                }
            };

            MoveState.Active = (StateMachine stateMachine) =>
            {
                if (m_MovementVector == Vector2.zero)
                {
                    stateMachine.NextState(IdleState);
                }

            };

            this.MoveStateMachine = new StateMachine(IdleState);

            MoveStateMachine.PreTick = () =>
            {
                m_MovementVector = m_InputActions.PlayerMovement.Move.ReadValue<Vector2>().normalized;
                if (math.abs(m_MovementVector.x) < 0.2f) m_MovementVector = Vector2.zero;
                m_MovementVector.y = 0;
            };

        }

        void InitialiseJumpStateMachine()
        {
            State GroundedState = new State(JUMP_GROUNDED);
            State FallingState = new State(JUMP_FALLING);
            State JumpState = new State(JUMP_JUMPING);
            State DoubleJumpState = new State(JUMP_DOUBLEJUMP);
            State SlamState = new State(JUMP_SLAM);

            GroundedState.Active = (StateMachine stateMachine) =>
            {
                if (!m_IsGrounded)
                {
                    stateMachine.NextState(FallingState);
                }

                if (m_InputActions.PlayerMovement.Jump.triggered)
                {
                    stateMachine.NextState(JumpState);
                }


            };

            JumpState.Enter = () =>
            {
                m_Rb.velocity = new Vector2(m_Rb.velocity.x, 0);
                m_Rb.AddForce(transform.up * m_JumpForce, ForceMode2D.Impulse);
                var go = Instantiate(m_JumpVfx, m_FeetLocation.position, Quaternion.identity);
                Destroy(go, 2f);

                groundedInhibitTime = 0;
            };

            JumpState.Active = (StateMachine stateMachine) =>
            {
                groundedInhibitTime += Time.deltaTime;
                if (m_InputActions.PlayerMovement.Slam.triggered)
                {
                    stateMachine.NextState(SlamState);
                }

                if (m_InputActions.PlayerMovement.Jump.triggered)
                {
                    stateMachine.NextState(DoubleJumpState);
                }

                if (m_IsGrounded && groundedInhibitTime > 0.4f)
                {
                    stateMachine.NextState(GroundedState);
                }
            };

            SlamState.Enter = () =>
            {
                m_Rb.AddForce(-transform.up * m_SlamForce, ForceMode2D.Impulse);
            };

            SlamState.Active = (StateMachine stateMachine) =>
            {

                if (m_IsGrounded)
                {
                    stateMachine.NextState(GroundedState);
                }
            };

            DoubleJumpState.Enter = () =>
            {
                m_Rb.velocity = new Vector2(m_Rb.velocity.x, 0);
                m_Rb.AddForce(transform.up * m_JumpForce, ForceMode2D.Impulse);
                var go = Instantiate(m_JumpVfx, m_FeetLocation.position, Quaternion.identity);
                Destroy(go, 2f);
            };

            DoubleJumpState.Active = (StateMachine stateMachine) =>
            {
                if (m_InputActions.PlayerMovement.Slam.triggered)
                {
                    stateMachine.NextState(SlamState);
                }

                if (m_IsGrounded)
                {
                    stateMachine.NextState(GroundedState);
                }
            };

            FallingState.Active = (StateMachine stateMachine) =>
            {

                if (m_InputActions.PlayerMovement.Jump.triggered)
                {
                    stateMachine.NextState(DoubleJumpState);
                }

                if (m_InputActions.PlayerMovement.Slam.triggered)
                {
                    stateMachine.NextState(SlamState);
                }

                if (m_IsGrounded)
                {
                    stateMachine.NextState(GroundedState);
                }
            };

            this.JumpStateMachine = new StateMachine(GroundedState);
        }

        void Start()
        {
            m_InputActions = new DefaultInputActions();
            m_InputActions.PlayerMovement.Enable();
            m_Rb = GetComponent<Rigidbody2D>();
            m_Col = GetComponent<BoxCollider2D>();
            m_AttributeSystem = GetComponent<AttributeSystemComponent>();
            m_AbilitySystemCharacter = GetComponent<AbilitySystemCharacter>();

            groundedMask = LayerMask.GetMask("Ground");
            m_AimTarget = GetComponent<AimTarget>();
            InitialiseMovementStateMachine();
            InitialiseJumpStateMachine();
        }

        void Update()
        {
            if (m_AttributeSystem.GetAttributeValue(m_SpeedAttribute, out var attributeValue))
            {
                m_MovementSpeed = attributeValue.CurrentValue;
            }

            m_IsGrounded = IsGrounded();
            MoveStateMachine.TickState();

            if (m_InputActions.PlayerMovement.Dash.triggered)
            {
                var dashSpec = this.m_DashAbility.CreateSpec(m_AbilitySystemCharacter);
                m_AbilitySystemCharacter.ActivateAbility(dashSpec);
            }


            if (!JumpStateMachine.IsState(JUMP_GROUNDED)) m_MovementVector = m_MovementVector / 1.2f;


            SetAimTargetFlip();
            //HandleJump();
            UpdatePlayerState();
            //HandleVFX();

            JumpStateMachine.TickState();
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

        void UpdatePlayerState()
        {
            this.m_PlayerState = PlayerStates.Idle;


            if (m_MovementVector.magnitude > 0)
            {
                this.m_PlayerState |= PlayerStates.Moving;
            }


        }

        public PlayerStates GetState()
        {
            return this.m_PlayerState;
        }

        bool IsGrounded()
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(m_Col.bounds.center, -transform.up, m_Col.bounds.extents.y + m_GroundTolerance, groundedMask);
            return raycastHit.collider != null;
        }

        void FixedUpdate()
        {
            if (MoveStateMachine.IsState(MOVE_NORMAL))
            {
                var movementVelocity = m_MovementVector * m_MovementSpeed;
                m_Rb.velocity = new Vector2(movementVelocity.x, m_Rb.velocity.y);
            }
        }
    }

}
