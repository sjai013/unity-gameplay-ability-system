using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityStandardAssets.CrossPlatformInput;
using static InputSystem.InputSystem;

namespace UnityStandardAssets.Cameras {
    public class FreeLookCam : PivotBasedCameraRig {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        [SerializeField] public float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)] [SerializeField] public float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
        [SerializeField] public float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [SerializeField] public float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
        [SerializeField] public float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
        [SerializeField] public bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
        [SerializeField] public bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return

        public float m_LookAngle;                    // The rig's y axis rotation.
        public float m_TiltAngle;                    // The pivot's x axis rotation.
        public const float k_LookDistance = 20f;    // How far in front of the pivot the character's look target is.
        public Vector3 m_PivotEulers;
        public Quaternion m_PivotTargetRot;
        public Quaternion m_TransformTargetRot;
        public InputSystem.InputSystem controls;

        protected override void Awake() {
            base.Awake();
            // Lock or unlock the cursor.
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_LockCursor;
            m_PivotEulers = m_Pivot.rotation.eulerAngles;

            m_PivotTargetRot = m_Pivot.transform.localRotation;
            m_TransformTargetRot = transform.localRotation;

            if (controls == null) {
                controls = new InputSystem.InputSystem();
            }
            controls.Movement.Enable();

        }
        public void FixedUpdate() {
            var look = controls.Movement.Look.ReadValue<Vector2>();
            HandleRotation(look.x, look.y);
        }


        protected override void FollowTarget(float deltaTime) {
            if (m_Target == null) return;
            // Move the rig towards target position.
            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);
        }

        private void HandleRotation(float x, float y) {
            if (Time.timeScale < float.Epsilon)
                return;

            // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            this.m_LookAngle += x * this.m_TurnSpeed;

            // Rotate the rig (the root object) around Y axis only:
            this.m_TransformTargetRot = Quaternion.Euler(0f, this.m_LookAngle, 0f);

            if (this.m_VerticalAutoReturn) {
                // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
                // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
                // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
                this.m_TiltAngle = y > 0 ? Mathf.Lerp(0, -this.m_TiltMin, y) : Mathf.Lerp(0, this.m_TiltMax, -y);
            } else {
                // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
                this.m_TiltAngle -= y * this.m_TurnSpeed;
                // and make sure the new value is within the tilt range
                this.m_TiltAngle = Mathf.Clamp(this.m_TiltAngle, -this.m_TiltMin, this.m_TiltMax);
            }

            // Tilt input around X is applied to the pivot (the child of this object)
            this.m_PivotTargetRot = Quaternion.Euler(this.m_TiltAngle, this.m_PivotEulers.y, this.m_PivotEulers.z);

            if (this.m_TurnSmoothing > 0) {
                this.m_Pivot.localRotation = Quaternion.Slerp(this.m_Pivot.localRotation, this.m_PivotTargetRot, this.m_TurnSmoothing * Time.deltaTime);
                this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, this.m_TransformTargetRot, this.m_TurnSmoothing * Time.deltaTime);
            } else {
                this.m_Pivot.localRotation = this.m_PivotTargetRot;
                this.transform.localRotation = this.m_TransformTargetRot;
            }
        }
    }
}
