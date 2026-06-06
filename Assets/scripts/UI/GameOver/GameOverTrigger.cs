using UnityEngine;
using VarelaAloisio.Core;
using UI;

public class GameOverTrigger :  MacacoBehaviour
{
    [SerializeField] private string gameOverMenuId = "Menu_GameOver_View";

    public void TriggerGameOver()
    {
        Navigator navigator = FindAnyObjectByType<Navigator>();

        if (navigator != null)
        {
            navigator.SendMessage("SwitchMenu", gameOverMenuId, SendMessageOptions.DontRequireReceiver);
            Log("Se envió la señal de apertura al Navigator con éxito.");
        }
        else
        {
            LogError("No se encontró el Navigator en la escena de UI. ¡Asegurate de que ambas escenas estén cargadas en modo Play!");
        }
    }
}
