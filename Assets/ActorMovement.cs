/*
 * Created on Mon Dec 23 2019
 *
 * The MIT License (MIT)
 * Copyright (c) 2019 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using UnityEngine;

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
