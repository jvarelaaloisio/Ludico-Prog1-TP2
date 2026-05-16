using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VarelaAloisio.Core.Editor
{
    public class ToolbarCustomizer
    {
        [MainToolbarElement("Play/From Boot", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement AddPlayFromBoot()
        {
            var content = new MainToolbarContent((Texture2D)EditorGUIUtility.IconContent("preAudioAutoPlayOff").image,
                                                 "Play from Boot");
            var button = new MainToolbarButton(content, PlayFromBoot);
            return button;
        }

        private static void PlayFromBoot()
        {
            if (EditorApplication.isPlaying)
            {
                //Thought: I just added this to see if a shotgun-to-the-chest kinda reset would work, but this is not intended to live for too long
                //(he said, knowing full well he would never touch this code ever again).
                SceneManager.LoadScene(ToolbarProjectSettings.BootScenePath, LoadSceneMode.Single);
                return;
            }
            SceneHelper.StartScene(ToolbarProjectSettings.BootScenePath);
        }
    }
}
