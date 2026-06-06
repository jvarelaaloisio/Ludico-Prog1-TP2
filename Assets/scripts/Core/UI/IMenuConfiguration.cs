using System;
using UnityEngine;

namespace Core.UI
{
    public interface IMenuConfiguration
    {
        /// <summary /> Instantiates all buttons into the menu
        /// <param name="parent"></param>
        /// <param name="switchTo"></param>
        void Setup(Transform parent, Action<string> switchTo);
    }
}