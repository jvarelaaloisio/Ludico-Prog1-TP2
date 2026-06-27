using System;
using UnityEngine;

namespace Core.UI
{
    public interface IMenu
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        Transform ButtonsParent { get; }
        void Open();
        void Close();
        void Setup(Action<string> switchTo);
        /// <summary /> If this is the menu for the given ID
        /// <param name="id">The ID for the menu being sought for.</param>
        bool Is(string id);
    }
}