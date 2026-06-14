using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VarelaAloisio.Core;

namespace Player
{
    public class SimpleCameraMaskCheat : MacacoBehaviour
    {
        [SerializeField] private InputActionReference cycleModesInput;
        [SerializeField] private new Camera camera;
        [SerializeField] private List<LayerMask> modes;
        private int _currentMode = 0;
        protected override void Start()
        {
            base.Start();
            modes.Insert(0, camera.cullingMask);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (cycleModesInput)
            {
                cycleModesInput.action.Enable();
                cycleModesInput.action.performed += Cycle;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (cycleModesInput)
            {
                cycleModesInput.action.Disable();
                cycleModesInput.action.performed -= Cycle;
            }
        }

        private void Cycle(InputAction.CallbackContext data)
            => camera.cullingMask = modes[(int)Mathf.Repeat(++_currentMode, modes.Count)];
    }
}