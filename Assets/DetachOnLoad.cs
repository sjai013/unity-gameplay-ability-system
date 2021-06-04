using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
