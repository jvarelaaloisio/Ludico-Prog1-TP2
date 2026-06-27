using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using VarelaAloisio.Core;

namespace UI
{
    public class TutorialController : MacacoBehaviour
    {
        [SerializeField] private InputIconsHandler[] handlers;
        [SerializeField] private InputActionReference[] analogInputs;

        private IDisposable _buttonPressSubscription;
        protected override void OnEnable()
        {
            base.OnEnable();
            _buttonPressSubscription = InputSystem.onAnyButtonPress.Call(DetectDeviceAndUpdate);
            foreach (InputActionReference input in analogInputs)
                input.action.started += HandleInput;
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();
            _buttonPressSubscription?.Dispose();
            foreach (InputActionReference input in analogInputs)
                input.action.started -= HandleInput;
        }

        private void DetectDeviceAndUpdate(InputControl control)
            => SetDevice(control.device);

        private void HandleInput(InputAction.CallbackContext context)
            => SetDevice(context.control.device);

        private void SetDevice(InputDevice device)
        {
            foreach (InputIconsHandler handler in handlers)
                handler.SetVersion(device);
        }
    }
}
