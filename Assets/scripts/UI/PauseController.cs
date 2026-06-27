using Core.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VarelaAloisio.Core;

namespace UI
{
    public class PauseController : MacacoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject defaultSelection;
        [SerializeField] private InputActionReference pauseAction;

        private bool _isPaused = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            pauseAction.action.Enable();
            pauseAction.action.performed += HandlePause;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            pauseAction.action.Disable();
            pauseAction.action.performed -= HandlePause;
        }

        private void HandlePause(InputAction.CallbackContext data)
            => TogglePause();

        private void TogglePause()
        {
            _isPaused = !_isPaused;
            
            pausePanel.SetActive(_isPaused);
            if (_isPaused)
                EventSystem.current.SetSelectedGameObject(defaultSelection);

            Time.timeScale = _isPaused ? 0f : 1f;
        }

        public void ResumeGame()
        {
            if (_isPaused)
                TogglePause();
        }

        public void GoToMainMenu()
        {
            if (Service.TryGet(out IGameManager gameManager))
            {
                gameManager.HandlePlayerDeath();
                Time.timeScale = 1f;
            }
            else
                LogError($"GameManager not found");
        }
    }
}