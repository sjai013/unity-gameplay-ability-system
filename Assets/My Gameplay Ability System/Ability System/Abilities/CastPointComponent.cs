using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class CastPointComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform CastPoint;
    public AbilityController controller;
    public Animator animator;
    private int attackIdxHash = Animator.StringToHash("AttackIdx");


    public Vector3 GetPosition()
    {
        var attackIdx = animator.GetInteger(attackIdxHash);
        this.CastPoint = controller.CastPoint[attackIdx];

        return CastPoint.position;
    }
}
