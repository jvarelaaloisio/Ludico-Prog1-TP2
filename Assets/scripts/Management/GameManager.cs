using System;
using System.Collections.Generic;
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
        [Serializable]
        private struct LevelState
        {
            [field: SerializeField] public Ref<ILevel> Level { get; private set; }
            [field: SerializeField] public GameState State { get; private set; }
        }

        [SerializeField] private List<LevelState> levels = new ();
        [SerializeField] private Ref<ILevel> defaultLevel;
        [SerializeField, SerializeReadOnly] private int currentLevel = 0;

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

            _levelService.OnLevelLoaded += HandleLevelLoaded;
            _levelService.LoadLevel(defaultLevel.Value);
        }

        private void HandleLevelLoaded(string levelName)
        {
            if (levelName != defaultLevel.Value.name)
                return;

            State = GameState.Menu;
            OnStateChange?.Invoke(State);
        }

        public void GoToNextLevel()
        {
            if (levels.Count > currentLevel
                && levels[currentLevel].Level.HasValue)
                _levelService.UnloadLevel(levels[currentLevel].Level.Value);
            SetConfig(levels[(int)Mathf.Repeat(++currentLevel, levels.Count)]);
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

                GoToNextLevel();
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
            SetConfig(levels[currentLevel = 0]);
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
