using System;
using UnityEngine;

namespace VarelaAloisio.Scenes
{
    [Serializable]
    public struct Chunk
    {
        [SerializeField]
        public int[] sceneIndexes;
		
        public int activeScene;
    }
}