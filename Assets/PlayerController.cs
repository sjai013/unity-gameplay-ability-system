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
    private bool shouldMove;

    private CharacterController controller;
    private Transform cameraTransform;

    [SerializeField]
    private AbilityController abilityController;

    private int mAnimator_ShouldMove = Animator.StringToHash("ShouldMove");
    private int mAnimator_Movement_X = Animator.StringToHash("Movement_X");
    private int mAnimator_Movement_Y = Animator.StringToHash("Movement_Y");

    private bool ability1Released = false;
    private bool ability2Released = false;


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
        this.mAnimator.SetBool(mAnimator_ShouldMove, this.shouldMove);

        this.mAnimator.SetFloat(this.mAnimator_Movement_X, this.mMovementVector.x);
        this.mAnimator.SetFloat(this.mAnimator_Movement_Y, this.mMovementVector.y);


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
        this.shouldMove = true;
        if (this.mMovementVector.magnitude < 0.2)
        {
            this.mMovementVector = Vector2.zero;
            this.shouldMove = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CaptureInputs();
        HandleMovement();
        HandleAbilityUse();
        ResetInputs();


    }

    private void HandleAbilityUse()
    {
        if (ability1Released)
        {
            this.abilityController.UseAbility(0);
        }

        if (ability2Released)
        {
            this.abilityController.UseAbility(1);
        }
    }

    private void ResetInputs()
    {
        ability1Released = false;
        ability2Released = false;
        shouldMove = false;
        this.mMovementVector = Vector2.zero;
    }

    public void OnFire1(InputAction.CallbackContext context)
    {
        if (context.performed) ability1Released = true;
    }

    public void OnFire2(InputAction.CallbackContext context)
    {
        if (context.performed) ability2Released = true;

    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        this.mMovementVector = context.ReadValue<Vector2>();
        this.shouldMove = true;
        if (this.mMovementVector.magnitude < 0.2)
        {
            this.mMovementVector = Vector2.zero;

        }

    }
}
