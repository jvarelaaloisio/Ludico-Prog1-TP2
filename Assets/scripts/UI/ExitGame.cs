using VarelaAloisio.Core;
using Core;

namespace UI
{
    public class ExitGame : MacacoBehaviour
    {
        public void Exit()
        {
            if (Service.TryGet(out IGameManager gameManager))
            {
                gameManager.ExitGame();
            }
            else
            {
                LogError("GameManager service not found.");
            }
        }
    }
}