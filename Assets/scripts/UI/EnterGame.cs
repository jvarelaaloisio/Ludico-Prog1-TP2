using VarelaAloisio.Core;
using Core;
using Core.Game;

namespace UI
{
    public class EnterGame : MacacoBehaviour
    {
        public void Enter()
        {
            if (Service.TryGet(out IGameManager gameManager))
            {
                gameManager.GoToNextLevel();
            }
            else
            {
                LogError("GameManager service not found.");
            }
        }
    }
}