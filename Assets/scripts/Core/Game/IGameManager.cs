using System;

namespace Core.Game
{
    public interface IGameManager
    {
        event Action OnLose;
        void EnterGame();
        void ExitGame();
        void HandlePlayerDeath();
        event Action<string> OnEnterLevel;
    }
}