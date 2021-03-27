using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, DefaultInputActions.IPlayerActions
{
    private DefaultInputActions playerInput;
    public void OnFire(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
    }

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new DefaultInputActions();
        playerInput.Enable();
        playerInput.Player.SetCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
