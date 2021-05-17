using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetFinderRay : MonoBehaviour, DefaultInputActions.IPlayerAbilitiesActions
{
    public void OnFire1(InputAction.CallbackContext context)
    {
        Debug.Log("Ability 1");
    }

    public void OnFire2(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity))
        {
            Debug.DrawLine(transform.position, hit.transform.position, Color.yellow);
        }
    }
}
