using System.Collections.Generic;

namespace VarelaAloisio.Scenes
{
    public interface ISceneBatch
    {
        /// <summary /> Scene Count.
        public abstract int Length { get; }

        /// <summary /> Called on Unity's OnValidate.
        public virtual void Validate() { }

        /// <summary /> Returns all async operations which load the batch.
        /// <returns> A collection of <see cref="SceneAsyncOperation"/>, one for each scene.</returns>
        public abstract IEnumerable<SceneAsyncOperation> Load();

        /// <summary /> Returns all async operations which unload the batch.
        /// <returns> A collection of <see cref="SceneAsyncOperation"/>, one for each scene.</returns>
        public abstract IEnumerable<SceneAsyncOperation> Unload();
    }
}