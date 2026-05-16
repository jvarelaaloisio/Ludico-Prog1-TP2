using UnityEngine;

namespace VarelaAloisio.Core.Utils
{
    public static class GameUtils
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
#endif
            Application.Quit();
        }
    }
}
