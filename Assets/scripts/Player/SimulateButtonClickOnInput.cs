using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VarelaAloisio.Core;

namespace Player
{
    public class SimulateButtonClickOnInput : MacacoBehaviour
    {
        [SerializeField] private InputActionReference input;
        [SerializeField] private Button button;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (input)
            {
                input.action.Enable();
                input.action.started += HandleInput;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (input)
            {
                input.action.Disable();
                input.action.started -= HandleInput;
            }
        }

        private void HandleInput(InputAction.CallbackContext context)
        {
            if (button)
                button.onClick.Invoke();
            else
                LogError("Button is null");
        }
    }
}