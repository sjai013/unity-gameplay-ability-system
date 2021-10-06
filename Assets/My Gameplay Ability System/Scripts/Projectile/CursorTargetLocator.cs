using GameplayAbilitySystemDemo.Input;
using UnityEngine;

namespace MyGameplayAbilitySystem.SampleAbilities
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [AddComponentMenu("Gameplay Ability System/Abilities/Cursor Targetting")]
    public class CursorTargetLocator : MonoBehaviour
    {
        DefaultInputActions m_InputActions;
        private Camera m_Cam;
        [SerializeField] private Vector2 m_CursorWorldPosition;

        void Start()
        {
            m_InputActions = new DefaultInputActions();
            m_InputActions.PlayerLook.Enable();
            m_Cam = Camera.main;
        }
        void Update()
        {
            m_CursorWorldPosition = m_Cam.ScreenToWorldPoint(m_InputActions.PlayerLook.Position.ReadValue<Vector2>());
        }

        public Vector2 GetCursorWorldPosition()
        {
            return m_CursorWorldPosition;
        }
    }
}