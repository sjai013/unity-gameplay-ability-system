using GameplayAbilitySystem;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem {

    protected override void OnUpdate() {
        Entities.ForEach((InputManager inputManager) => {
            for (var i = 0; i < inputManager.InputLength(); i++) {
                bool triggerEvent = false;
                var input = inputManager.GetElementAt(i);
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

public struct MouseScreenPosition : IComponentData {
    public float x;
    public float y;
}

public struct MouseDelta : IComponentData {
    public float x;
    public float y;
}