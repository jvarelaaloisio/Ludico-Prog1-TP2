using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadLevel(string sceneName)
    {
        Debug.Log("Cargando la escena: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}