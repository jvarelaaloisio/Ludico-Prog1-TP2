using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInput;

        [SerializeField] private float moveSpeed = 5f;
        private Rigidbody2D _rigidBody;

        private void Awake()
            => _rigidBody = GetComponent<Rigidbody2D>();

        private void OnEnable()
        {
            if (moveInput)
            {
                moveInput.action.Enable();
                moveInput.action.started += HandleMoveInput;
                moveInput.action.performed += HandleMoveInput;
                moveInput.action.canceled += HandleMoveInput;
            }
        }

        private void HandleMoveInput(InputAction.CallbackContext data)
        {
            var direction = data.ReadValue<Vector2>();
            _rigidBody.linearVelocity = direction * moveSpeed;
        }

        private void OnDisable()
        {
            if (_rigidBody)
                _rigidBody.linearVelocity = Vector2.zero;
            if (moveInput)
            {
                moveInput.action.Disable();
                moveInput.action.started -= HandleMoveInput;
                moveInput.action.performed -= HandleMoveInput;
                moveInput.action.canceled -= HandleMoveInput;
            }
        }
    }
}
