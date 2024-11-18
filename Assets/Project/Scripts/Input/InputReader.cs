using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Platformer
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };

        PlayerInputActions inputAction;

        public Vector2 Direction => inputAction.Player.Move.ReadValue<Vector2>();

        private void OnEnable()
        {
            if(inputAction == null)
            {
                inputAction = new PlayerInputActions();
                inputAction.Player.SetCallbacks(this);
            }
            inputAction.Enable();
        }
        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";
        public void EnablePlayerActions() => inputAction.Enable();
        public void OnFire(InputAction.CallbackContext context)
        {
            //No-op
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump?.Invoke(false);
                    break;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera?.Invoke();
                    break;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            //No-op
        }
    }
}
