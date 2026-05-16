using System.Collections.Generic;

namespace VarelaAloisio.Scenes
{
    public interface ILevel
    {
        string name { get; }
        SerializableScene ActiveScene { get; }
        /// <summary /> Total number of scenes in the level. Counting immediate and deferred batches.
        int TotalSceneCount { get; }

        /// <summary /> The batch of scenes to load immediately.
        /// <remarks> These will be loaded while loading screen is being shown.</remarks>
        /// <p> Prevents entering the level.</p>
        ISceneBatch ImmediateBatch { get; }

        /// <summary /> Loads the immediate batch of the level.
        /// <returns> A collection of <see cref="SceneAsyncOperation"/>, one for each scene.</returns>
        IEnumerable<SceneAsyncOperation> LoadImmediate();

        /// <summary /> Loads the deferred batches of the level.
        /// <returns> A collection of <see cref="SceneAsyncOperation"/>, one for each scene.</returns>
        IEnumerable<ISceneBatch> LoadDeferredBatches { get; }

        /// <summary /> Unloads the level.
        /// <returns> A collection of <see cref="SceneAsyncOperation"/>, one for each scene.</returns>
        IEnumerable<SceneAsyncOperation> Unload();
    }
}