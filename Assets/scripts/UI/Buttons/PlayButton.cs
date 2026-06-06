using Core;
using UnityEngine;
using UnityEngine.UI;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI.Buttons
{
    public class PlayButton : MacacoBehaviour
    {
        [AutoMap(How.Service, When.OnEnable)]
        private IGameManager _gameManager;

        [SerializeField] private Button button;

        protected override void Reset()
        {
            base.Reset();
            button = GetComponent<Button>()
                     ?? gameObject.AddComponent<Button>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            button.onClick.AddListener(HandleClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
            => _gameManager.EnterGame();
    }
}