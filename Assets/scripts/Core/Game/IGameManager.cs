using System;
using System.Threading.Tasks;

namespace Core.Game
{
    public interface IGameManager
    {
        event Action OnLose;
        event Action OnWinLevel;
        void GoToNextLevel();
        void ExitGame();
        void HandlePlayerDeath();
        event Action<string> OnEnterLevel;
        Task WinLevel(float delayBeforeGoingBackToMenu);
        event Action<GameState> OnStateChange;
    }
}