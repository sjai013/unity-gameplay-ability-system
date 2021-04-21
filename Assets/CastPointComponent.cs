using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPointComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    protected Transform CastPoint;

    public Vector3 GetPosition()
    {
        return CastPoint.transform.position;
    }
}
