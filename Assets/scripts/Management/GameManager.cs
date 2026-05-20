using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Scenes;

namespace Management
{
    public class GameManager : MacacoBehaviour
    {
        [SerializeField] private Ref<ILevel> defaultLevel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
            => Service.Flush();

        protected override void Start()
        {
            base.Start();
            if (!Service.TryGet(out ILevelService levelService))
            {
                LogError($"{nameof(levelService)} not found.");
                return;
            }

            if (!defaultLevel.HasValue)
            {
                LogError($"{nameof(defaultLevel)} is null.");
                return;
            }
            levelService.LoadLevel(defaultLevel.Value);
        }
    }
}
