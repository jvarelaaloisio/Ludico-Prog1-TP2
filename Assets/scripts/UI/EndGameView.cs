using System;
using Core.Game;
using TMPro;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI
{
    public class EndGameView : MacacoBehaviour
    {
        [AutoMap(How.Service, When.OnEnable)]
        private IGameManager _gameManager;

        [SerializeField] private float fadeInDuration = 1;
        [SerializeField] private TMP_Text label;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_gameManager is not null)
            {
                _gameManager.OnWinLevel += HandleWin;
            }
        }

        private async void HandleWin()
        {
            try
            {
                float start = Time.time;
                float now = 0;
                do
                {
                    now = Time.time;
                    float lerp = (now - start) / fadeInDuration;
                    label.alpha = lerp;
                    await Awaitable.NextFrameAsync();
                } while (now < start + fadeInDuration
                         && !DisableCancellationToken.IsCancellationRequested);
            }
            catch (Exception e) { LogException(e); }
        }
    }
}
