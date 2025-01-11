//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Settings/TWHGInputActions.inputactions
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

namespace SansyHuman.TWHG.Core
{
    public partial class @TWHGInputActions: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @TWHGInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""TWHGInputActions"",
    ""maps"": [
        {
            ""name"": ""Editor"",
            ""id"": ""8ea1478d-6ffe-4a67-8e8d-14d868d17965"",
            ""actions"": [
                {
                    ""name"": ""PointerPosition"",
                    ""type"": ""Value"",
                    ""id"": ""a5aeac2a-b139-4b4e-856f-066c1b41ed7f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ScreenClick"",
                    ""type"": ""Button"",
                    ""id"": ""8049265e-e021-4f23-9416-849125c97304"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScreenNegClick"",
                    ""type"": ""Button"",
                    ""id"": ""6b59f66a-e26d-4d9c-8e2a-458c9f66c378"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScreenMag"",
                    ""type"": ""Value"",
                    ""id"": ""24155618-3b78-43e7-8ec2-2e23f9792c4b"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""81a9bf74-d707-4e82-b9e6-4e1ca3188b70"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Alt"",
                    ""type"": ""Button"",
                    ""id"": ""c35a32a1-e05f-4209-813d-7105f3752876"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ctrl"",
                    ""type"": ""Button"",
                    ""id"": ""dae9687b-072b-4074-b5a4-40580feb8cb6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shift"",
                    ""type"": ""Button"",
                    ""id"": ""0df89ab2-634a-4f59-932e-756b9cb0346f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9e1ffaf4-ac72-4293-8958-5ea2ab0bbf14"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87ca4396-53b3-4d1c-8bd8-bb927c9ed5c1"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""ScreenClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d0ef3049-80a0-4241-a20d-d057904f47d6"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""ScreenMag"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""14dd85d8-8e17-4e22-9349-43307fee03ee"",
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
                    ""id"": ""873b5d50-19e2-4fab-9d43-e8de1464f23e"",
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
                    ""id"": ""07050cae-2ec1-4eb1-ae1f-22bdcaa84050"",
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
                    ""id"": ""f3a46daa-710e-4f1d-aefa-a70157f62d80"",
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
                    ""id"": ""5685331a-4d7d-46e0-8060-3770e76c75a7"",
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
                    ""id"": ""72a21f98-4cdf-46f9-887d-deee6cf9ff69"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""ScreenNegClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea1ae2e7-b9e9-4086-a84a-f0421d9afbec"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Alt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a309fcc6-a500-4efd-ae23-bc6b0cc68c11"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Ctrl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ea25053-de51-47b0-adea-91ee007f500b"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Shift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Game"",
            ""id"": ""0cbedc15-efc2-4d29-a445-829ac0175bb3"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b296f33c-e8d7-4785-9bf1-1d875a1eff0d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""6a8f8fa1-6fca-4644-80e7-525b99375414"",
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
                    ""id"": ""755bef44-2304-4b99-bbd7-37107c69b8e5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""65f9b7f1-7419-4128-9c91-6c00efc419d5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bd565286-9fd7-4569-b02b-4e70f2c3b8f0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3b1498f4-ff1d-49db-9c51-57ea2c320699"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow"",
                    ""id"": ""6b0db163-66eb-4b3f-ac4d-44deeecbdb9a"",
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
                    ""id"": ""832698f8-1bb1-4bd0-af67-00df61b1a986"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0e492206-dd8b-4cb6-9a6a-58faa185c498"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""84f7d570-973b-4276-a45c-6b208693c670"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""60eb2077-cb3f-4560-9ce4-4e48bd9af655"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Editor
            m_Editor = asset.FindActionMap("Editor", throwIfNotFound: true);
            m_Editor_PointerPosition = m_Editor.FindAction("PointerPosition", throwIfNotFound: true);
            m_Editor_ScreenClick = m_Editor.FindAction("ScreenClick", throwIfNotFound: true);
            m_Editor_ScreenNegClick = m_Editor.FindAction("ScreenNegClick", throwIfNotFound: true);
            m_Editor_ScreenMag = m_Editor.FindAction("ScreenMag", throwIfNotFound: true);
            m_Editor_Move = m_Editor.FindAction("Move", throwIfNotFound: true);
            m_Editor_Alt = m_Editor.FindAction("Alt", throwIfNotFound: true);
            m_Editor_Ctrl = m_Editor.FindAction("Ctrl", throwIfNotFound: true);
            m_Editor_Shift = m_Editor.FindAction("Shift", throwIfNotFound: true);
            // Game
            m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
            m_Game_Move = m_Game.FindAction("Move", throwIfNotFound: true);
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

        // Editor
        private readonly InputActionMap m_Editor;
        private List<IEditorActions> m_EditorActionsCallbackInterfaces = new List<IEditorActions>();
        private readonly InputAction m_Editor_PointerPosition;
        private readonly InputAction m_Editor_ScreenClick;
        private readonly InputAction m_Editor_ScreenNegClick;
        private readonly InputAction m_Editor_ScreenMag;
        private readonly InputAction m_Editor_Move;
        private readonly InputAction m_Editor_Alt;
        private readonly InputAction m_Editor_Ctrl;
        private readonly InputAction m_Editor_Shift;
        public struct EditorActions
        {
            private @TWHGInputActions m_Wrapper;
            public EditorActions(@TWHGInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @PointerPosition => m_Wrapper.m_Editor_PointerPosition;
            public InputAction @ScreenClick => m_Wrapper.m_Editor_ScreenClick;
            public InputAction @ScreenNegClick => m_Wrapper.m_Editor_ScreenNegClick;
            public InputAction @ScreenMag => m_Wrapper.m_Editor_ScreenMag;
            public InputAction @Move => m_Wrapper.m_Editor_Move;
            public InputAction @Alt => m_Wrapper.m_Editor_Alt;
            public InputAction @Ctrl => m_Wrapper.m_Editor_Ctrl;
            public InputAction @Shift => m_Wrapper.m_Editor_Shift;
            public InputActionMap Get() { return m_Wrapper.m_Editor; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(EditorActions set) { return set.Get(); }
            public void AddCallbacks(IEditorActions instance)
            {
                if (instance == null || m_Wrapper.m_EditorActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_EditorActionsCallbackInterfaces.Add(instance);
                @PointerPosition.started += instance.OnPointerPosition;
                @PointerPosition.performed += instance.OnPointerPosition;
                @PointerPosition.canceled += instance.OnPointerPosition;
                @ScreenClick.started += instance.OnScreenClick;
                @ScreenClick.performed += instance.OnScreenClick;
                @ScreenClick.canceled += instance.OnScreenClick;
                @ScreenNegClick.started += instance.OnScreenNegClick;
                @ScreenNegClick.performed += instance.OnScreenNegClick;
                @ScreenNegClick.canceled += instance.OnScreenNegClick;
                @ScreenMag.started += instance.OnScreenMag;
                @ScreenMag.performed += instance.OnScreenMag;
                @ScreenMag.canceled += instance.OnScreenMag;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Alt.started += instance.OnAlt;
                @Alt.performed += instance.OnAlt;
                @Alt.canceled += instance.OnAlt;
                @Ctrl.started += instance.OnCtrl;
                @Ctrl.performed += instance.OnCtrl;
                @Ctrl.canceled += instance.OnCtrl;
                @Shift.started += instance.OnShift;
                @Shift.performed += instance.OnShift;
                @Shift.canceled += instance.OnShift;
            }

            private void UnregisterCallbacks(IEditorActions instance)
            {
                @PointerPosition.started -= instance.OnPointerPosition;
                @PointerPosition.performed -= instance.OnPointerPosition;
                @PointerPosition.canceled -= instance.OnPointerPosition;
                @ScreenClick.started -= instance.OnScreenClick;
                @ScreenClick.performed -= instance.OnScreenClick;
                @ScreenClick.canceled -= instance.OnScreenClick;
                @ScreenNegClick.started -= instance.OnScreenNegClick;
                @ScreenNegClick.performed -= instance.OnScreenNegClick;
                @ScreenNegClick.canceled -= instance.OnScreenNegClick;
                @ScreenMag.started -= instance.OnScreenMag;
                @ScreenMag.performed -= instance.OnScreenMag;
                @ScreenMag.canceled -= instance.OnScreenMag;
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Alt.started -= instance.OnAlt;
                @Alt.performed -= instance.OnAlt;
                @Alt.canceled -= instance.OnAlt;
                @Ctrl.started -= instance.OnCtrl;
                @Ctrl.performed -= instance.OnCtrl;
                @Ctrl.canceled -= instance.OnCtrl;
                @Shift.started -= instance.OnShift;
                @Shift.performed -= instance.OnShift;
                @Shift.canceled -= instance.OnShift;
            }

            public void RemoveCallbacks(IEditorActions instance)
            {
                if (m_Wrapper.m_EditorActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IEditorActions instance)
            {
                foreach (var item in m_Wrapper.m_EditorActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_EditorActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public EditorActions @Editor => new EditorActions(this);

        // Game
        private readonly InputActionMap m_Game;
        private List<IGameActions> m_GameActionsCallbackInterfaces = new List<IGameActions>();
        private readonly InputAction m_Game_Move;
        public struct GameActions
        {
            private @TWHGInputActions m_Wrapper;
            public GameActions(@TWHGInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Game_Move;
            public InputActionMap Get() { return m_Wrapper.m_Game; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
            public void AddCallbacks(IGameActions instance)
            {
                if (instance == null || m_Wrapper.m_GameActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_GameActionsCallbackInterfaces.Add(instance);
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }

            private void UnregisterCallbacks(IGameActions instance)
            {
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
            }

            public void RemoveCallbacks(IGameActions instance)
            {
                if (m_Wrapper.m_GameActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IGameActions instance)
            {
                foreach (var item in m_Wrapper.m_GameActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_GameActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public GameActions @Game => new GameActions(this);
        private int m_PCSchemeIndex = -1;
        public InputControlScheme PCScheme
        {
            get
            {
                if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
                return asset.controlSchemes[m_PCSchemeIndex];
            }
        }
        public interface IEditorActions
        {
            void OnPointerPosition(InputAction.CallbackContext context);
            void OnScreenClick(InputAction.CallbackContext context);
            void OnScreenNegClick(InputAction.CallbackContext context);
            void OnScreenMag(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnAlt(InputAction.CallbackContext context);
            void OnCtrl(InputAction.CallbackContext context);
            void OnShift(InputAction.CallbackContext context);
        }
        public interface IGameActions
        {
            void OnMove(InputAction.CallbackContext context);
        }
    }
}
