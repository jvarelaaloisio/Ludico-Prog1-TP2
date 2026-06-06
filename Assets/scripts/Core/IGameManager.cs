using System;

namespace Core
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