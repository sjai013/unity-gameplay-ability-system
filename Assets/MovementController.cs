using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private Transform cam;

    private float turnSmoothVelocity;
    private DefaultInputActions playerInput;


    void Awake()
    {
        playerInput = new DefaultInputActions();
        playerInput.Enable();
    }
    // Update is called once per frame
    void Update()
    {
        var movementVector = CaptureMovementFromInput();

        if (movementVector.magnitude >= 0.1f)
        {
            float lookAngle = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, lookAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, lookAngle, 0f).normalized * Vector3.forward;
            controller.Move(moveDirection * speed * Time.deltaTime);
        }
    }

    Vector3 CaptureMovementFromInput()
    {
        var movementVector = this.playerInput.PlayerMovement.Move.ReadValue<Vector2>().normalized;
        return new Vector3(movementVector.x, 0f, movementVector.y);
    }
}
