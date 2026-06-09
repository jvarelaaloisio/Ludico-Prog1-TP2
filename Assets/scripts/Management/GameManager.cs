using System;
using Core;
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
