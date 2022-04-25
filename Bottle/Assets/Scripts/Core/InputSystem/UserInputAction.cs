// GENERATED AUTOMATICALLY FROM 'Assets/Resources/InputSystem/UserInputAction.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Bottle.Core.InputSystem
{
    public class @UserInputAction : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @UserInputAction()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""UserInputAction"",
    ""maps"": [
        {
            ""name"": ""Editor"",
            ""id"": ""08c55e15-b656-4b6a-9826-bf6525f65e46"",
            ""actions"": [
                {
                    ""name"": ""RotateAroundYAxis"",
                    ""type"": ""PassThrough"",
                    ""id"": ""64ad78ab-1955-4b9e-aecf-c2cc7c514283"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c7a6fc19-bc59-4bc4-9200-3027f1539fe7"",
                    ""path"": ""<Mouse>/scroll/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateAroundYAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b06eb01-f010-470a-8255-349f87ebaa8b"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateAroundYAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""InGame"",
            ""id"": ""8ded49cc-3589-402f-8237-67adfafd6ca4"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""PassThrough"",
                    ""id"": ""69a35d74-1bbf-479d-b896-91746aa9dd71"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""de5ed404-0379-4570-8745-afb4b77fb6fe"",
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
                    ""id"": ""31a196ed-2009-40dd-935f-2b2df7c7bf61"",
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
                    ""id"": ""0bf2697a-3553-4730-9328-262d5be71c8a"",
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
                    ""id"": ""c4c7227e-5c9f-481f-8349-bee96803f12e"",
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
                    ""id"": ""d9e4f313-1400-42ed-9252-736664334fb8"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Editor
            m_Editor = asset.FindActionMap("Editor", throwIfNotFound: true);
            m_Editor_RotateAroundYAxis = m_Editor.FindAction("RotateAroundYAxis", throwIfNotFound: true);
            // InGame
            m_InGame = asset.FindActionMap("InGame", throwIfNotFound: true);
            m_InGame_Move = m_InGame.FindAction("Move", throwIfNotFound: true);
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

        // Editor
        private readonly InputActionMap m_Editor;
        private IEditorActions m_EditorActionsCallbackInterface;
        private readonly InputAction m_Editor_RotateAroundYAxis;
        public struct EditorActions
        {
            private @UserInputAction m_Wrapper;
            public EditorActions(@UserInputAction wrapper) { m_Wrapper = wrapper; }
            public InputAction @RotateAroundYAxis => m_Wrapper.m_Editor_RotateAroundYAxis;
            public InputActionMap Get() { return m_Wrapper.m_Editor; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(EditorActions set) { return set.Get(); }
            public void SetCallbacks(IEditorActions instance)
            {
                if (m_Wrapper.m_EditorActionsCallbackInterface != null)
                {
                    @RotateAroundYAxis.started -= m_Wrapper.m_EditorActionsCallbackInterface.OnRotateAroundYAxis;
                    @RotateAroundYAxis.performed -= m_Wrapper.m_EditorActionsCallbackInterface.OnRotateAroundYAxis;
                    @RotateAroundYAxis.canceled -= m_Wrapper.m_EditorActionsCallbackInterface.OnRotateAroundYAxis;
                }
                m_Wrapper.m_EditorActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @RotateAroundYAxis.started += instance.OnRotateAroundYAxis;
                    @RotateAroundYAxis.performed += instance.OnRotateAroundYAxis;
                    @RotateAroundYAxis.canceled += instance.OnRotateAroundYAxis;
                }
            }
        }
        public EditorActions @Editor => new EditorActions(this);

        // InGame
        private readonly InputActionMap m_InGame;
        private IInGameActions m_InGameActionsCallbackInterface;
        private readonly InputAction m_InGame_Move;
        public struct InGameActions
        {
            private @UserInputAction m_Wrapper;
            public InGameActions(@UserInputAction wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_InGame_Move;
            public InputActionMap Get() { return m_Wrapper.m_InGame; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(InGameActions set) { return set.Get(); }
            public void SetCallbacks(IInGameActions instance)
            {
                if (m_Wrapper.m_InGameActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_InGameActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_InGameActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_InGameActionsCallbackInterface.OnMove;
                }
                m_Wrapper.m_InGameActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                }
            }
        }
        public InGameActions @InGame => new InGameActions(this);
        public interface IEditorActions
        {
            void OnRotateAroundYAxis(InputAction.CallbackContext context);
        }
        public interface IInGameActions
        {
            void OnMove(InputAction.CallbackContext context);
        }
    }
}
