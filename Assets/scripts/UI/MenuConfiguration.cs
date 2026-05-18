using System;
using Core.UI;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "UI/Menu", fileName = "Menu_", order = 0)]
    [Serializable]
    public class MenuConfiguration : ScriptableObject, IMenuConfiguration
    {
        [SerializeField] private RectTransform menuPrefab;

        /// <summary /> Instantiates all buttons into the menu
        /// <param name="parent"></param>
        /// <param name="SwitchTo"></param>
        public void Setup(Transform parent, Action<string> SwitchTo)
        {
            RectTransform menu = Instantiate(menuPrefab, parent);
        }
    }
}