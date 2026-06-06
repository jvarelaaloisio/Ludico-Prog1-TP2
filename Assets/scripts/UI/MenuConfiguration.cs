using System;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [CreateAssetMenu(menuName = "UI/Menu", fileName = "Menu_", order = 0)]
    [Serializable]
    public class MenuConfiguration : ScriptableObject, IMenuConfiguration
    {
        [SerializeField] private RectTransform menuPrefab;

        /// <summary /> Instantiates all buttons into the menu
        /// <param name="parent"></param>
        /// <param name="switchTo"></param>
        public void Setup(Transform parent, Action<string> switchTo)
        {
            RectTransform menu = Instantiate(menuPrefab, parent);
            menu.localScale = Vector3.one;

            // busca todos los NavButton que haya adentro de este menú recién creado
            NavButton[] navButtons = menu.GetComponentsInChildren<NavButton>();

            // los recorro
            foreach (var navBtn in navButtons)
            {
                Button btn = navBtn.GetComponent<Button>();
                btn.onClick.AddListener(() => switchTo(navBtn.targetMenuId));
            }
        }
    }
}