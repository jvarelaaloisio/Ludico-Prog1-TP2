using UnityEngine;
using VarelaAloisio.Core;

namespace UI
{
    public class TutorialController : MacacoBehaviour
    {
        [SerializeField] private GameObject tutorialCanvas;

        private void Start()
        {
            ShowTutorial();
        }

        private void ShowTutorial()
        {
            if (tutorialCanvas != null)
            {
                tutorialCanvas.SetActive(true);

                Time.timeScale = 0f;
            }
        }

        public void CloseTutorial()
        {
            if (tutorialCanvas != null)
            {
                tutorialCanvas.SetActive(false);
                
                Time.timeScale = 1f;
            }
        }
    }
}
