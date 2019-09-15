using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Cameras
{
    public class FreeLookCam : PivotBasedCameraRig
    {
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

        protected override void Awake()
        {
            base.Awake();
            // Lock or unlock the cursor.
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_LockCursor;
			m_PivotEulers = m_Pivot.rotation.eulerAngles;

	        m_PivotTargetRot = m_Pivot.transform.localRotation;
			m_TransformTargetRot = transform.localRotation;
        }


        protected override void FollowTarget(float deltaTime)
        {
            if (m_Target == null) return;
            // Move the rig towards target position.
            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime*m_MoveSpeed);
        }


      
    }
}
