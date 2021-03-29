using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, DefaultInputActions.IPlayerActions
{
    [SerializeField]
    private float playerSpeed = 2f;

    [SerializeField]
    private bool isGrounded;

    [SerializeField]
    private float rotationSpeed = 5;
    private DefaultInputActions playerInput;
    private Animator mAnimator;

    private Vector2 mMovementVector;
    private bool mMoving;

    private CharacterController controller;
    private Transform cameraTransform;

    public void OnFire(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new DefaultInputActions();
        playerInput.Enable();
        playerInput.Player.SetCallbacks(this);
        this.mAnimator = GetComponent<Animator>();
        this.controller = GetComponent<CharacterController>();
        this.cameraTransform = Camera.main.transform;
    }

    void HandleMovement()
    {
        this.isGrounded = this.controller.isGrounded;
        this.mAnimator.SetBool("Moving", this.mMoving);
        this.mAnimator.SetFloat("XSpeed", this.mMovementVector.x);
        this.mAnimator.SetFloat("YSpeed", this.mMovementVector.y);


        var movementVector = new Vector3(mMovementVector.x, 0, mMovementVector.y);
        movementVector = cameraTransform.forward * movementVector.z + cameraTransform.right * movementVector.x;
        movementVector.y = 0;
        this.controller.Move(movementVector * Time.deltaTime * playerSpeed);

        if (mMovementVector.y != 0)
        {
            var targetAngle = math.degrees(math.atan2(mMovementVector.x, math.abs(mMovementVector.y))) + cameraTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    void CaptureInputs()
    {
        this.mMovementVector = this.playerInput.Player.Move.ReadValue<Vector2>();
        this.mMoving = true;
        if (this.mMovementVector.magnitude < 0.2)
        {
            this.mMovementVector = Vector2.zero;
            this.mMoving = false;
        }



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CaptureInputs();
        HandleMovement();
    }
}
