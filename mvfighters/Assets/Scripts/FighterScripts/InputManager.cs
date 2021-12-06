//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Scripts/FighterScripts/InputManager.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputManager : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""Combat1"",
            ""id"": ""16629fc3-1f87-4de2-b5f3-dd3983d55b46"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""6d7f1177-59ff-4173-9cae-460d5f29c433"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""A"",
                    ""type"": ""Button"",
                    ""id"": ""b7395630-4c9b-4187-905d-28f0ceb2fd54"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""B"",
                    ""type"": ""Button"",
                    ""id"": ""ef9f6011-b15f-4101-a85b-913e2c9208a1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""C"",
                    ""type"": ""Button"",
                    ""id"": ""2541ca43-b0c6-45ec-944b-4c89b9abe155"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a4bb40aa-b828-4343-8ea7-5b3f299a6b31"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""bb703c00-cada-40b3-b6f7-3b41b3a21ccc"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""cf9e9389-095b-4dfd-ab6e-31c6d5a538d1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""cec32b3b-56a0-4e46-b106-bdec4b2975d4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""1187bf48-dd7e-44a7-b0ab-27d7ca080c52"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8f8230e8-0a32-440d-b027-b3bb4e759bdb"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea19276f-009f-4082-b5e3-68da58cfcebc"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48bc0825-05a1-4fd3-93c2-2e02fdfc0e12"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""C"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Combat2"",
            ""id"": ""49d09f08-1b6d-486f-a7aa-298d95b8473a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""2ebb0f88-f6a4-4999-b39d-8182a620ed41"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""A"",
                    ""type"": ""Button"",
                    ""id"": ""710e6a28-a9f5-4775-9ea7-0aca0ed9a428"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""B"",
                    ""type"": ""Button"",
                    ""id"": ""cb7b27da-3e3e-442d-bc60-5cceb8b7f868"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""C"",
                    ""type"": ""Button"",
                    ""id"": ""956adda6-8c74-4a5a-aae3-1df416be5df7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""39bfb223-5b0d-45e3-bb8d-c0d157e8bed1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4502193f-4ec5-45b2-8393-50199c1e268b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""fd183e2b-86a8-475b-bbf0-a9248b8388ba"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f7431848-7838-4ec0-ae27-f00e81412053"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8df5d1e2-8d35-4eab-8ba9-a378d9faa567"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e1c665d6-0dcc-4794-8fef-2c433c8f1992"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38e55f5e-d524-4a2c-88ee-9df72afaf5cc"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b62050ac-6b0a-4a43-a3aa-70d8d3c9dbba"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""C"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyBoard"",
            ""bindingGroup"": ""KeyBoard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Combat1
        m_Combat1 = asset.FindActionMap("Combat1", throwIfNotFound: true);
        m_Combat1_Move = m_Combat1.FindAction("Move", throwIfNotFound: true);
        m_Combat1_A = m_Combat1.FindAction("A", throwIfNotFound: true);
        m_Combat1_B = m_Combat1.FindAction("B", throwIfNotFound: true);
        m_Combat1_C = m_Combat1.FindAction("C", throwIfNotFound: true);
        // Combat2
        m_Combat2 = asset.FindActionMap("Combat2", throwIfNotFound: true);
        m_Combat2_Move = m_Combat2.FindAction("Move", throwIfNotFound: true);
        m_Combat2_A = m_Combat2.FindAction("A", throwIfNotFound: true);
        m_Combat2_B = m_Combat2.FindAction("B", throwIfNotFound: true);
        m_Combat2_C = m_Combat2.FindAction("C", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Combat1
    private readonly InputActionMap m_Combat1;
    private ICombat1Actions m_Combat1ActionsCallbackInterface;
    private readonly InputAction m_Combat1_Move;
    private readonly InputAction m_Combat1_A;
    private readonly InputAction m_Combat1_B;
    private readonly InputAction m_Combat1_C;
    public struct Combat1Actions
    {
        private @InputManager m_Wrapper;
        public Combat1Actions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Combat1_Move;
        public InputAction @A => m_Wrapper.m_Combat1_A;
        public InputAction @B => m_Wrapper.m_Combat1_B;
        public InputAction @C => m_Wrapper.m_Combat1_C;
        public InputActionMap Get() { return m_Wrapper.m_Combat1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Combat1Actions set) { return set.Get(); }
        public void SetCallbacks(ICombat1Actions instance)
        {
            if (m_Wrapper.m_Combat1ActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnMove;
                @A.started -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnA;
                @A.performed -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnA;
                @A.canceled -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnA;
                @B.started -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnB;
                @B.performed -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnB;
                @B.canceled -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnB;
                @C.started -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnC;
                @C.performed -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnC;
                @C.canceled -= m_Wrapper.m_Combat1ActionsCallbackInterface.OnC;
            }
            m_Wrapper.m_Combat1ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @A.started += instance.OnA;
                @A.performed += instance.OnA;
                @A.canceled += instance.OnA;
                @B.started += instance.OnB;
                @B.performed += instance.OnB;
                @B.canceled += instance.OnB;
                @C.started += instance.OnC;
                @C.performed += instance.OnC;
                @C.canceled += instance.OnC;
            }
        }
    }
    public Combat1Actions @Combat1 => new Combat1Actions(this);

    // Combat2
    private readonly InputActionMap m_Combat2;
    private ICombat2Actions m_Combat2ActionsCallbackInterface;
    private readonly InputAction m_Combat2_Move;
    private readonly InputAction m_Combat2_A;
    private readonly InputAction m_Combat2_B;
    private readonly InputAction m_Combat2_C;
    public struct Combat2Actions
    {
        private @InputManager m_Wrapper;
        public Combat2Actions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Combat2_Move;
        public InputAction @A => m_Wrapper.m_Combat2_A;
        public InputAction @B => m_Wrapper.m_Combat2_B;
        public InputAction @C => m_Wrapper.m_Combat2_C;
        public InputActionMap Get() { return m_Wrapper.m_Combat2; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Combat2Actions set) { return set.Get(); }
        public void SetCallbacks(ICombat2Actions instance)
        {
            if (m_Wrapper.m_Combat2ActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnMove;
                @A.started -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnA;
                @A.performed -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnA;
                @A.canceled -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnA;
                @B.started -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnB;
                @B.performed -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnB;
                @B.canceled -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnB;
                @C.started -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnC;
                @C.performed -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnC;
                @C.canceled -= m_Wrapper.m_Combat2ActionsCallbackInterface.OnC;
            }
            m_Wrapper.m_Combat2ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @A.started += instance.OnA;
                @A.performed += instance.OnA;
                @A.canceled += instance.OnA;
                @B.started += instance.OnB;
                @B.performed += instance.OnB;
                @B.canceled += instance.OnB;
                @C.started += instance.OnC;
                @C.performed += instance.OnC;
                @C.canceled += instance.OnC;
            }
        }
    }
    public Combat2Actions @Combat2 => new Combat2Actions(this);
    private int m_KeyBoardSchemeIndex = -1;
    public InputControlScheme KeyBoardScheme
    {
        get
        {
            if (m_KeyBoardSchemeIndex == -1) m_KeyBoardSchemeIndex = asset.FindControlSchemeIndex("KeyBoard");
            return asset.controlSchemes[m_KeyBoardSchemeIndex];
        }
    }
    public interface ICombat1Actions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnA(InputAction.CallbackContext context);
        void OnB(InputAction.CallbackContext context);
        void OnC(InputAction.CallbackContext context);
    }
    public interface ICombat2Actions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnA(InputAction.CallbackContext context);
        void OnB(InputAction.CallbackContext context);
        void OnC(InputAction.CallbackContext context);
    }
}
