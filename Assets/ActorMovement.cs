using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ActorMovement : MonoBehaviour {
    public float moveSpeed;
    public InputSystem.InputSystem controls;
    Rigidbody rb;

    Camera cam;
    void Awake() {
        rb = GetComponent<Rigidbody>();
        if (controls == null) {
            controls = new InputSystem.InputSystem();
        }
        controls.Movement.Enable();

        cam = Camera.main;
    }

    void Update() {
        FaceCameraDirection();
        Move();
    }

    private void Move() {
        var move = controls.Movement.WASD.ReadValue<Vector2>();
        var MoveVector = (move.y * cam.transform.forward + move.x * cam.transform.right).normalized;
        MoveVector *= Time.deltaTime;
        MoveVector *= moveSpeed;
        MoveVector.y = 0;
        rb.MovePosition(transform.position + MoveVector);
    }

    private void FaceCameraDirection() {
        var rotationAngle = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        rb.MoveRotation(rotationAngle);
    }
}
