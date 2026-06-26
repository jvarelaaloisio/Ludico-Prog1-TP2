using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VarelaAloisio.Scenes
{
    [Serializable]
    public class SceneBatch : ISceneBatch
    {
        [field: SerializeField] public List<SerializableScene> Scenes { get; private set; } = new();

        /// <inheritdoc />
        public int Length => Scenes.Count;

        /// <inheritdoc />
        public void Validate()
        {
            foreach (SerializableScene scene in Scenes)
                scene.Validate();
        }

        /// <inheritdoc />
        public IEnumerable<SceneAsyncOperation> Load()
            => Scenes
               .Where(scene => !scene.IsPersistent || !SceneManager.GetSceneByBuildIndex(scene.BuildIndex).isLoaded)
               .Select(scene => scene.BuildIndex)
               .Select(i => new SceneAsyncOperation(SceneUtility.GetScenePathByBuildIndex(i),
                                                    SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive)));

        /// <inheritdoc />
        public IEnumerable<SceneAsyncOperation> Unload()
            => Scenes.Where(scene => !scene.IsPersistent)
                     .Select(scene => scene.BuildIndex)
                     .Where(i => SceneManager.GetSceneByBuildIndex(i).isLoaded)
                     .Select(i => new SceneAsyncOperation(SceneUtility.GetScenePathByBuildIndex(i),
                                                          SceneManager.UnloadSceneAsync(i)));
    }
}