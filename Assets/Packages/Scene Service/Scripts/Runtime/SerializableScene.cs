using System;
using VarelaAloisio.Core.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VarelaAloisio.Scenes
{
    [Serializable]
    public class SerializableScene
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset scene;
#endif
        [SerializeField, SerializeReadOnly] private int buildIndex = -1;
        [field: SerializeField] public bool IsPersistent { get; private set; }
        public int BuildIndex => buildIndex;
        public string Path => buildIndex >= 0
                                  ? SceneUtility.GetScenePathByBuildIndex(buildIndex)
                                  : "Invalid";

        public void Validate()
        {
#if UNITY_EDITOR
            if (scene)
                buildIndex = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(scene));
            else
                buildIndex = -1;
#endif
        }

        public bool HasScene()
            => buildIndex >= 0 && buildIndex < SceneManager.sceneCountInBuildSettings;

        public override string ToString()
        {
#if UNITY_EDITOR
            return scene?.ToString() ?? "null";
#else
            return base.ToString();
#endif
        }
    }
}