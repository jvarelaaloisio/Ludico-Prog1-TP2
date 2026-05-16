using UnityEditor;
using UnityEditor.SceneManagement;

namespace VarelaAloisio.Core.Editor
{
    public static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return;
                
            if(EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneToOpen) != null)
                {
                    EditorSceneManager.OpenScene(sceneToOpen);
                    EditorApplication.isPlaying = true;
                }
            }
            sceneToOpen = null;
        }
    }
}