// GENERATED AUTOMATICALLY FROM 'Assets/Input System/InputSystem.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace InputSystem
{
    public class @InputSystem : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public @InputSystem()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputSystem"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""ee44a2e2-69b6-425c-8d91-daac50648d72"",
            ""actions"": [
                {
                    ""name"": ""WASD"",
                    ""type"": ""Button"",
                    ""id"": ""90c51565-156c-4ea4-9ef1-e57ed3c070cc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Button"",
                    ""id"": ""1346cc8a-d455-407a-aa83-ce3be306d361"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""23b275b3-ff88-4281-b219-c307bbcc7d99"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""7c1552ae-993b-4626-9a92-73ff2aa4af4a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c622d713-5281-445d-a49f-021a90e302a6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9948a193-2af4-46cb-bb6d-e18674f5e974"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""933a95f9-be58-4b14-a79c-0f7bda631b7a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""53154254-0d73-491a-8e6d-010daa15a63c"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=0.5,y=0.5)"",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Cast"",
            ""id"": ""f7575aa1-89ac-40d1-a531-3b1a5c87142e"",
            ""actions"": [
                {
                    ""name"": "" Cast 1"",
                    ""type"": ""Button"",
                    ""id"": ""e3563fa9-0ea2-4684-b9c5-51666e2be096"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 2"",
                    ""type"": ""Button"",
                    ""id"": ""6194ec09-fc60-471c-8374-44549ee6d983"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 3"",
                    ""type"": ""Button"",
                    ""id"": ""f6032960-7560-4baa-a09e-c0ddaa7a97fb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 4"",
                    ""type"": ""Button"",
                    ""id"": ""ab83b989-8efb-4824-85c1-9894a31041c9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 5"",
                    ""type"": ""Button"",
                    ""id"": ""ae9221c7-d4ab-4a72-8528-e760bd2d8852"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 6"",
                    ""type"": ""Button"",
                    ""id"": ""21c2b4d8-3d7e-4dc4-9bc4-87c3cecf7a5d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 7"",
                    ""type"": ""Button"",
                    ""id"": ""e5cf2634-205c-4ec8-b141-46a1353779bc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": "" Cast 8"",
                    ""type"": ""Button"",
                    ""id"": ""1798181d-9c90-4385-9663-1952e722d850"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""cfa47c03-5a40-4c04-b2a3-74805d84c3e5"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4214ae3-10aa-4537-9464-321c0332d529"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e68656d-1875-4ccb-a29b-872beec34852"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04a1506b-4abf-4241-a4d1-37cc2b6923f9"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4ba20fbe-dc05-402c-980a-7ed55aadcbc8"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""91927efd-c7b9-4aaa-9bb8-dc95a8fb5e0a"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1d83f16b-66fb-4f20-a2ca-98deb3f9b261"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""519cf2c9-e9ab-42c8-9497-1e48c36a441e"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": "" Cast 8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KBM"",
            ""bindingGroup"": ""KBM"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Movement
            m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
            m_Movement_WASD = m_Movement.FindAction("WASD", throwIfNotFound: true);
            m_Movement_Look = m_Movement.FindAction("Look", throwIfNotFound: true);
            // Cast
            m_Cast = asset.FindActionMap("Cast", throwIfNotFound: true);
            m_Cast_Cast1 = m_Cast.FindAction(" Cast 1", throwIfNotFound: true);
            m_Cast_Cast2 = m_Cast.FindAction(" Cast 2", throwIfNotFound: true);
            m_Cast_Cast3 = m_Cast.FindAction(" Cast 3", throwIfNotFound: true);
            m_Cast_Cast4 = m_Cast.FindAction(" Cast 4", throwIfNotFound: true);
            m_Cast_Cast5 = m_Cast.FindAction(" Cast 5", throwIfNotFound: true);
            m_Cast_Cast6 = m_Cast.FindAction(" Cast 6", throwIfNotFound: true);
            m_Cast_Cast7 = m_Cast.FindAction(" Cast 7", throwIfNotFound: true);
            m_Cast_Cast8 = m_Cast.FindAction(" Cast 8", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Movement
        private readonly InputActionMap m_Movement;
        private IMovementActions m_MovementActionsCallbackInterface;
        private readonly InputAction m_Movement_WASD;
        private readonly InputAction m_Movement_Look;
        public struct MovementActions
        {
            private @InputSystem m_Wrapper;
            public MovementActions(@InputSystem wrapper) { m_Wrapper = wrapper; }
            public InputAction @WASD => m_Wrapper.m_Movement_WASD;
            public InputAction @Look => m_Wrapper.m_Movement_Look;
            public InputActionMap Get() { return m_Wrapper.m_Movement; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
            public void SetCallbacks(IMovementActions instance)
            {
                if (m_Wrapper.m_MovementActionsCallbackInterface != null)
                {
                    @WASD.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnWASD;
                    @WASD.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnWASD;
                    @WASD.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnWASD;
                    @Look.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnLook;
                }
                m_Wrapper.m_MovementActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @WASD.started += instance.OnWASD;
                    @WASD.performed += instance.OnWASD;
                    @WASD.canceled += instance.OnWASD;
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                }
            }
        }
        public MovementActions @Movement => new MovementActions(this);

        // Cast
        private readonly InputActionMap m_Cast;
        private ICastActions m_CastActionsCallbackInterface;
        private readonly InputAction m_Cast_Cast1;
        private readonly InputAction m_Cast_Cast2;
        private readonly InputAction m_Cast_Cast3;
        private readonly InputAction m_Cast_Cast4;
        private readonly InputAction m_Cast_Cast5;
        private readonly InputAction m_Cast_Cast6;
        private readonly InputAction m_Cast_Cast7;
        private readonly InputAction m_Cast_Cast8;
        public struct CastActions
        {
            private @InputSystem m_Wrapper;
            public CastActions(@InputSystem wrapper) { m_Wrapper = wrapper; }
            public InputAction @Cast1 => m_Wrapper.m_Cast_Cast1;
            public InputAction @Cast2 => m_Wrapper.m_Cast_Cast2;
            public InputAction @Cast3 => m_Wrapper.m_Cast_Cast3;
            public InputAction @Cast4 => m_Wrapper.m_Cast_Cast4;
            public InputAction @Cast5 => m_Wrapper.m_Cast_Cast5;
            public InputAction @Cast6 => m_Wrapper.m_Cast_Cast6;
            public InputAction @Cast7 => m_Wrapper.m_Cast_Cast7;
            public InputAction @Cast8 => m_Wrapper.m_Cast_Cast8;
            public InputActionMap Get() { return m_Wrapper.m_Cast; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CastActions set) { return set.Get(); }
            public void SetCallbacks(ICastActions instance)
            {
                if (m_Wrapper.m_CastActionsCallbackInterface != null)
                {
                    @Cast1.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast1;
                    @Cast1.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast1;
                    @Cast1.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast1;
                    @Cast2.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast2;
                    @Cast2.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast2;
                    @Cast2.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast2;
                    @Cast3.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast3;
                    @Cast3.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast3;
                    @Cast3.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast3;
                    @Cast4.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast4;
                    @Cast4.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast4;
                    @Cast4.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast4;
                    @Cast5.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast5;
                    @Cast5.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast5;
                    @Cast5.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast5;
                    @Cast6.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast6;
                    @Cast6.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast6;
                    @Cast6.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast6;
                    @Cast7.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast7;
                    @Cast7.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast7;
                    @Cast7.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast7;
                    @Cast8.started -= m_Wrapper.m_CastActionsCallbackInterface.OnCast8;
                    @Cast8.performed -= m_Wrapper.m_CastActionsCallbackInterface.OnCast8;
                    @Cast8.canceled -= m_Wrapper.m_CastActionsCallbackInterface.OnCast8;
                }
                m_Wrapper.m_CastActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Cast1.started += instance.OnCast1;
                    @Cast1.performed += instance.OnCast1;
                    @Cast1.canceled += instance.OnCast1;
                    @Cast2.started += instance.OnCast2;
                    @Cast2.performed += instance.OnCast2;
                    @Cast2.canceled += instance.OnCast2;
                    @Cast3.started += instance.OnCast3;
                    @Cast3.performed += instance.OnCast3;
                    @Cast3.canceled += instance.OnCast3;
                    @Cast4.started += instance.OnCast4;
                    @Cast4.performed += instance.OnCast4;
                    @Cast4.canceled += instance.OnCast4;
                    @Cast5.started += instance.OnCast5;
                    @Cast5.performed += instance.OnCast5;
                    @Cast5.canceled += instance.OnCast5;
                    @Cast6.started += instance.OnCast6;
                    @Cast6.performed += instance.OnCast6;
                    @Cast6.canceled += instance.OnCast6;
                    @Cast7.started += instance.OnCast7;
                    @Cast7.performed += instance.OnCast7;
                    @Cast7.canceled += instance.OnCast7;
                    @Cast8.started += instance.OnCast8;
                    @Cast8.performed += instance.OnCast8;
                    @Cast8.canceled += instance.OnCast8;
                }
            }
        }
        public CastActions @Cast => new CastActions(this);
        private int m_KBMSchemeIndex = -1;
        public InputControlScheme KBMScheme
        {
            get
            {
                if (m_KBMSchemeIndex == -1) m_KBMSchemeIndex = asset.FindControlSchemeIndex("KBM");
                return asset.controlSchemes[m_KBMSchemeIndex];
            }
        }
        public interface IMovementActions
        {
            void OnWASD(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
        }
        public interface ICastActions
        {
            void OnCast1(InputAction.CallbackContext context);
            void OnCast2(InputAction.CallbackContext context);
            void OnCast3(InputAction.CallbackContext context);
            void OnCast4(InputAction.CallbackContext context);
            void OnCast5(InputAction.CallbackContext context);
            void OnCast6(InputAction.CallbackContext context);
            void OnCast7(InputAction.CallbackContext context);
            void OnCast8(InputAction.CallbackContext context);
        }
    }
}
