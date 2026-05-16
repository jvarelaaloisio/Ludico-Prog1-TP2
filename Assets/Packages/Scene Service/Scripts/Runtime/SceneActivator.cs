using System.Collections;
using System.Linq;
using VarelaAloisio.Core.Extensions;
using VarelaAloisio.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using VarelaAloisio.Core.Runtime;

namespace VarelaAloisio.Scenes
{
    [AddComponentMenu("Scene Loader")]
    public class SceneActivator : MonoBehaviour
    {
        [Tooltip("The Scene loader will set active one parent each frame")]
        [SerializeField] private GameObject[] sceneParents;

        [Header("Batched Enabling")]
        [Tooltip("The Scene loader will enable all children of this GameObject in batches"
                 + "\nThis process starts after all sceneParents are enabled")]
        [SerializeField] private GameObject enableControlledGroup;
        [Tooltip("Number of GameObjects to enable per frame")]
        [SerializeField] private int enableBatchSize = 50;
        
        [Header("Logging")]
        [SerializeField] private Ref<ILogger> logger;
        [SerializeField] private string logTag = "SceneLoader";

        private void Start()
        {
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            var start = Time.realtimeSinceStartupAsDouble;
            logger.Log(logTag, $"Starting GO loading for {gameObject.scene.name.Colored("blue")}", this);
            foreach (var parent in sceneParents.Where(go => !go.activeSelf))
            {
                parent.SetActive(true);
                yield return null;
            }

            if (enableControlledGroup)
            {
                var batchCounter = 0;
                foreach (Transform child in enableControlledGroup.transform)
                {
                    child.gameObject.SetActive(true);
                    if (++batchCounter % enableBatchSize == 0)
                        yield return null;
                }
                logger.Log(logTag, $"{batchCounter} GOs were activated in the {gameObject.scene.name} scene" +
                                          $"\nin a span of {(batchCounter / enableBatchSize).Colored("red")} frames", this);
            }
            
            logger.Log(logTag, $"Scene activated in {(Time.realtimeSinceStartupAsDouble - start).Colored("green")} seconds", this);
            gameObject.SetActive(false);
        }
        
#if UNITY_EDITOR
        [MenuItem("GameObject/Scene Control/Scene Loader", false, 10)]
        private static void CreateCustomItem()
        {
            GameObject newObject = new GameObject("Scene Loader");

            newObject.AddComponent<SceneActivator>();

            Selection.activeGameObject = newObject;
            SceneView.lastActiveSceneView.FrameSelected();
        }
#endif
    }
}
