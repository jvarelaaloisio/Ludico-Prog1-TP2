using Core;
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
        }
        
        public void ExitGame()
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
    }
}
