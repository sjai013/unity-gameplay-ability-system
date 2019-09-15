using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class CameraSystem : ComponentSystem
{
    protected override void OnUpdate() {

        Entities.WithAll<FreeLookCam, CameraComponent, MouseDelta>().ForEach((Entity entity, FreeLookCam cam, ref CameraComponent camComponent, ref MouseDelta delta) => {
            delta.x = Input.GetAxis("Mouse X");
            delta.y = Input.GetAxis("Mouse Y");
            if (camComponent.isEnabled) HandleRotation(cam, delta.x, delta.y);
        });
    }

    private void HandleRotation(FreeLookCam cam, float x, float y) {
        if (Time.timeScale < float.Epsilon)
            return;

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        cam.m_LookAngle += x * cam.m_TurnSpeed;

        // Rotate the rig (the root object) around Y axis only:
        cam.m_TransformTargetRot = Quaternion.Euler(0f, cam.m_LookAngle, 0f);

        if (cam.m_VerticalAutoReturn) {
            // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
            // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
            // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
            cam.m_TiltAngle = y > 0 ? Mathf.Lerp(0, -cam.m_TiltMin, y) : Mathf.Lerp(0, cam.m_TiltMax, -y);
        } else {
            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            cam.m_TiltAngle -= y * cam.m_TurnSpeed;
            // and make sure the new value is within the tilt range
            cam.m_TiltAngle = Mathf.Clamp(cam.m_TiltAngle, -cam.m_TiltMin, cam.m_TiltMax);
        }

        // Tilt input around X is applied to the pivot (the child of this object)
        cam.m_PivotTargetRot = Quaternion.Euler(cam.m_TiltAngle, cam.m_PivotEulers.y, cam.m_PivotEulers.z);

        if (cam.m_TurnSmoothing > 0) {
            cam.m_Pivot.localRotation = Quaternion.Slerp(cam.m_Pivot.localRotation, cam.m_PivotTargetRot, cam.m_TurnSmoothing * Time.deltaTime);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, cam.m_TransformTargetRot, cam.m_TurnSmoothing * Time.deltaTime);
        } else {
            cam.m_Pivot.localRotation = cam.m_PivotTargetRot;
            cam.transform.localRotation = cam.m_TransformTargetRot;
        }
    }
}