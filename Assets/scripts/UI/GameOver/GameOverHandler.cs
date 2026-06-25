using Core;
using Core.Game;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI.GameOver
{
    public class GameOverHandler : MacacoBehaviour
    { 
        [AutoMap(How.Service, When.Start)]
        private IGameManager _gameManager;
        [AutoMap(How.Service, When.Start)]
        private IFuryManager _furyManager;

        public void RestartGame(){
            _furyManager?.ResetFury();
            _gameManager?.GoToNextLevel();
        }
    }
}
