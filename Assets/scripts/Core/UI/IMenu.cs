using System;
using UnityEngine;

namespace Core.UI
{
    public interface IMenu
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        IMenuConfiguration Configuration { get; }
        Transform ButtonsParent { get; }
        void Open();
        void Close();
    }
}