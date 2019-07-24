using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour {
    public static List<GameObject> ActiveInputComponents = new List<GameObject>();

    [SerializeField]
    private List<InputHandler> Inputs = new List<InputHandler>();

    // Start is called before the first frame update
    private void Awake() {
        if (!ActiveInputComponents.Find(x => x == this)) {
            ActiveInputComponents.Add(gameObject);
        }
    }

    public int InputLength() {
        return Inputs.Count;
    }

    public InputHandler GetElementAt(int i) {
        return Inputs[i];
    }
    //private void Update() {
    //    for (int i = 0; i < Inputs.Count; i++) {
    //        bool triggerEvent = false;
    //        var input = Inputs[i];
    //        if (input.ButtonState.HasFlag(EButtonState.ButtonDown) &&
    //            Input.GetButtonDown(input.Button)) triggerEvent = true;

    //        if (input.ButtonState.HasFlag(EButtonState.ButtonUp) &&
    //            Input.GetButtonUp(input.Button)) triggerEvent = true;

    //        if (input.ButtonState.HasFlag(EButtonState.ButtonHeld) &&
    //            Input.GetButton(input.Button)) triggerEvent = true;

    //        if (triggerEvent) {
    //            input.Handler?.Invoke();
    //        }
    //    }
    //}

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