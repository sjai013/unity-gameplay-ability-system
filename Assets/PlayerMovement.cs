using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    DefaultInputActions m_InputActions;
    Rigidbody2D m_Rb;
    BoxCollider2D m_Col;
    [SerializeField] private float m_GroundTolerance = 0.1f;
    [SerializeField] private float m_MovementSpeed = 1f;
    [SerializeField] private float m_JumpForce = 2f;
    private Vector2 m_JumpDirection;

    private Vector2 m_MovementVector;

    [SerializeField] private bool m_IsGrounded;

    // Start is called before the first frame update
    void Start()
    {
        m_InputActions = new DefaultInputActions();
        m_InputActions.PlayerMovement.Enable();
        m_Rb = GetComponent<Rigidbody2D>();
        m_Col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_MovementVector = m_InputActions.PlayerMovement.Move.ReadValue<Vector2>().normalized;
        m_IsGrounded = IsGrounded();

        if (m_MovementVector.magnitude < 0.2f) m_MovementVector = Vector2.zero;
        if (!m_IsGrounded) m_MovementVector = m_MovementVector / 1.2f;

        m_MovementVector.y = 0;
        HandleJump();
    }

    bool HandleJump()
    {
        var jumpTriggered = m_InputActions.PlayerMovement.Jump.triggered;
        if (jumpTriggered && m_IsGrounded)
        {
            m_Rb.AddForce(transform.up * m_JumpForce, ForceMode2D.Impulse);
            return true;
        }
        return false;
    }

    bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(m_Col.bounds.center, Vector2.down, m_Col.bounds.extents.y + m_GroundTolerance);
        Debug.DrawRay(m_Col.bounds.center, Vector2.down * (m_Col.bounds.extents.y + m_GroundTolerance), raycastHit.collider == null ? Color.red : Color.green);
        return raycastHit.collider != null;
    }

    void FixedUpdate()
    {
        var movementVelocity = (m_MovementVector + m_JumpDirection) * m_MovementSpeed * Time.deltaTime;
        m_Rb.velocity = new Vector2(movementVelocity.x, m_Rb.velocity.y);
    }
}
