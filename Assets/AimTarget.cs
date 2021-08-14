using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimTarget : MonoBehaviour
{
    [SerializeField] private Transform m_Aim;
    [SerializeField] private float m_MinRotation;
    [SerializeField] private float m_MaxRotation;
    [SerializeField] private float m_RotationSpeed;
    private Camera m_Cam;


    DefaultInputActions m_InputActions;

    // Start is called before the first frame update
    void Start()
    {
        m_InputActions = new DefaultInputActions();
        m_InputActions.PlayerLook.Enable();
        m_Cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = m_InputActions.PlayerLook.Look.ReadValue<Vector2>();
        Vector2 mousePositionWorld = m_Cam.ScreenToWorldPoint(mousePosition);
        Vector2 playerToMouseVector = mousePositionWorld - new Vector2(this.transform.position.x, this.transform.position.y);
        float targetLookAngle = math.clamp(Vector2.SignedAngle(transform.right, playerToMouseVector), m_MinRotation, m_MaxRotation);
        Quaternion targetLookQuaternion = Quaternion.Euler(0, 0, targetLookAngle);
        var deltaAngle = math.abs(Quaternion.Angle(m_Aim.transform.rotation, targetLookQuaternion));

        // If angle difference between current rotation and target rotation is small, clamp to target rotation.
        if (deltaAngle <= 1)
        {
            m_Aim.transform.rotation = targetLookQuaternion;
        }
        else
        {
            // Not close to target rotation, so lerp
            m_Aim.transform.rotation = Quaternion.Lerp(m_Aim.transform.rotation, targetLookQuaternion, Time.deltaTime * m_RotationSpeed);
        }

    }
}
