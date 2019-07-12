using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour {
    public static List<GameObject> ActiveInputComponents = new List<GameObject>();

    public List<InputHandler> Inputs = new List<InputHandler>();

    // Start is called before the first frame update
    private void Awake() {
        if (!ActiveInputComponents.Find(x => x == this)) {
            ActiveInputComponents.Add(gameObject);
        }
    }

    private void OnDestroy() {
        ActiveInputComponents.Remove(gameObject);
    }
}

[Serializable]
public struct InputHandler {
    public string Button;
    public EButtonState ButtonState;
    public UnityEvent Handler;
}

[Flags]
public enum EButtonState {
    ButtonDown = 0x01,
    ButtonUp = 0x02,
    ButtonHeld = 0x04
}