using Core.Game;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Components
{
    public class GoToNextLevel : MacacoBehaviour
    {
        [AutoMap(How.Service, When.Start)]
        private IGameManager _gameManager;

        public void Go()
            => _gameManager?.GoToNextLevel();
    }
}
