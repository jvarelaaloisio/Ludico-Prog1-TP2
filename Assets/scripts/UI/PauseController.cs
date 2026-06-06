using UnityEngine;
using VarelaAloisio.Core;

namespace UI
{
    public class PauseController : MacacoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private bool isPaused = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        private void TogglePause()
        {
            isPaused = !isPaused;
            
            pausePanel.SetActive(isPaused);

            Time.timeScale = isPaused ? 0f : 1f;
        }

        public void ResumeGame()
        {

            if (isPaused)
            {
                TogglePause();
            }
        }
    }
}