using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Game;
using UnityEngine;
using UnityEngine.Serialization;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Scenes;

namespace Management
{
    [Service(typeof(IGameManager))]
    public class GameManager : MacacoBehaviour, IGameManager
    {
        [Serializable]
        private struct LevelState
        {
            [field: SerializeField] public Ref<ILevel> Level { get; private set; }
            [field: SerializeField] public GameState State { get; private set; }
        }

        [SerializeField] private List<LevelState> levels = new ();
        [SerializeField] private Ref<ILevel> defaultLevel;
        [SerializeField, SerializeReadOnly] private int currentLevel = 0;
        [Obsolete]
        [FormerlySerializedAs("gameLevel")] [SerializeField] private Ref<ILevel> night1;
        [Obsolete]
        [SerializeField] private Ref<ILevel> night2;

        [AutoMap(How.Service, When.Start)]
        private ILevelService _levelService;

        [field: SerializeField, SerializeReadOnly] public GameState State { get; private set; }

        public event Action<GameState> OnStateChange;
        [Obsolete]
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

        public void GoToNextLevel()
            => SetConfig(levels[++currentLevel]);
 
        /// <inheritdoc />
        public async Task WinLevel(float delayBeforeGoingBackToMenu)
        {
            try
            {
                OnWinLevel?.Invoke();
                await Awaitable.WaitForSecondsAsync(delayBeforeGoingBackToMenu);
                if (DisableCancellationToken.IsCancellationRequested)
                    return;

                SetConfig(levels[++currentLevel]);
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

        private void SetConfig(LevelState config)
        {
            if (config.Level.HasValue)
            {
                _levelService.LoadLevel(config.Level.Value);
                OnEnterLevel?.Invoke(config.Level.Value.name);
            }
            else
                LogError($"Level config with state {config.State} has no level assigned");
            State = config.State;
            OnStateChange?.Invoke(config.State);
        }
    }
}
