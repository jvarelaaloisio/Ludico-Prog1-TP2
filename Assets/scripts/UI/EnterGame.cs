using VarelaAloisio.Core;
using Core;

namespace UI
{
    public class EnterGame : MacacoBehaviour
    {
        public void Enter()
        {
            if (Service.TryGet(out IGameManager gameManager))
            {
                gameManager.EnterGame();
            }
            else
            {
                LogError("GameManager service not found.");
            }
        }
    }
}