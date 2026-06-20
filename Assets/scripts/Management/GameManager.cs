using System;
using System.Threading.Tasks;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Scenes;

namespace Management
{
    [Service(typeof(IGameManager))]
    public class GameManager : MacacoBehaviour, IGameManager
    {
        [SerializeField] private Ref<ILevel> defaultLevel;
        [SerializeField] private Ref<ILevel> gameLevel;

        [AutoMap(How.Service, When.Start)]
        private ILevelService _levelService;

        public event Action<string> OnEnterLevel;

        public event Action OnLose;

        /// <inheritdoc />
        public event Action OnWinLevel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
            => Service.Flush();

        protected override void Start()
        {
            base.Start();

            if (!defaultLevel.HasValue)
            {
                LogError($"{nameof(defaultLevel)} is null.");
                return;
            }
            _levelService.LoadLevel(defaultLevel.Value);
        }

        public void EnterGame()
        {
            _levelService.LoadLevel(gameLevel.Value);
            OnEnterLevel?.Invoke(gameLevel.Value.name);
        }

        /// <inheritdoc />
        public async Task WinLevel(float delayBeforeGoingBackToMenu)
        {
            try
            {
                OnWinLevel?.Invoke();
                await Awaitable.WaitForSecondsAsync(delayBeforeGoingBackToMenu);
                if (DisableCancellationToken.IsCancellationRequested)
                    return;

                if (!defaultLevel.HasValue)
                {
                    LogError($"{nameof(defaultLevel)} is null.");
                    return;
                }
                _levelService.UnloadLevel(gameLevel.Value);
                OnEnterLevel?.Invoke(defaultLevel.Value.name);
            }
            catch (Exception e) { LogException(e); }
        }
        
        public void ExitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void HandlePlayerDeath()
        {
            OnLose?.Invoke();
        }
    }
}
