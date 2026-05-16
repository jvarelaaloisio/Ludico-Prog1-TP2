using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VarelaAloisio.Core.Extensions;

namespace VarelaAloisio.Scenes
{
    [CreateAssetMenu(fileName = "Level", menuName = "Models/Level", order = 0)]
    [Serializable]
    public class Level : ScriptableObject, ILevel
    {
        [field: Tooltip("Scene set as active. Must be previously loaded or be loaded in the immediate load batch")]
        [field: SerializeField]
        public SerializableScene ActiveScene { get; protected set; }

        [Tooltip("scenes to load immediately")]
        [SerializeField] private SceneBatch immediateBatch;

        [Space(25f)]
        [Tooltip("scenes that can be loaded after starting the game")]
        [SerializeField] private List<SceneBatch> deferredBatches;

        public ISceneBatch ImmediateBatch => immediateBatch;
        public IEnumerable<ISceneBatch> LoadDeferredBatches => deferredBatches;

        /// <inheritdoc />
        public virtual int TotalSceneCount
            => ImmediateBatch.Length + LoadDeferredBatches.Select(batch => batch.Length)
                                                          .DefaultIfEmpty(0)
                                                          .Aggregate((total, current) => total + current);

        private void OnValidate()
        {
            ImmediateBatch.Validate();
            foreach (var batch in LoadDeferredBatches)
                batch.Validate();
            ActiveScene.Validate();

            DefaultActiveSceneIfNull();
            return;

            void DefaultActiveSceneIfNull()
            {
                if (ActiveScene.HasScene()
                    || immediateBatch.Scenes is null or { Count: 0 })
                    return;
                SerializableScene defaultScene = immediateBatch.Scenes[0];

                if (!defaultScene.HasScene())
                    return;

                Debug.Log($"{name} ({nameof(Level).Colored(Color.gray7)}): No {nameof(ActiveScene)} set, defaulting to {defaultScene.Path} (first scene in {nameof(immediateBatch)})");
                ActiveScene = defaultScene;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        /// <summary /> Use to retrieve the List of Scenes to unload this level
        /// <returns>An enumerable of all the scenes to unload</returns>
        public virtual IEnumerable<SceneAsyncOperation> Unload()
            => ImmediateBatch.Unload()
                             .Concat(LoadDeferredBatches.SelectMany(batch => batch.Unload()));

        public virtual IEnumerable<SceneAsyncOperation> LoadImmediate()
            => ImmediateBatch.Load();
    }
}