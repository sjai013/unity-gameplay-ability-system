using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem {

    protected override void OnUpdate() {
        Entities.ForEach((InputManager inputManager) => {
            for (var i = 0; i < inputManager.Inputs.Count; i++) {
                bool triggerEvent = false;
                var input = inputManager.Inputs[i];
                if (input.ButtonState.HasFlag(EButtonState.ButtonDown) &&
                    Input.GetButtonDown(input.Button)) triggerEvent = true;

                if (input.ButtonState.HasFlag(EButtonState.ButtonUp) &&
                    Input.GetButtonUp(input.Button)) triggerEvent = true;

                if (input.ButtonState.HasFlag(EButtonState.ButtonHeld) &&
                    Input.GetButton(input.Button)) triggerEvent = true;

                if (triggerEvent) {
                    input.Handler?.Invoke();
                }
            }
        });
    }
}