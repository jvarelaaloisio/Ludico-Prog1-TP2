using System;
using System.Collections.Generic;
using Core.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VarelaAloisio.Core;

namespace UI
{
    public class Menu : MacacoBehaviour, IMenu
    {
        [field: SerializeField] public Transform ButtonsParent { get; set; }
        [Tooltip("Button/selectable which will be selected when this menu is opened")]
        [SerializeField] private GameObject defaultSelection;
        [SerializeField] private UnityEvent onOpen;
        [SerializeField] private UnityEvent onClose;

        public void Setup(Action<string> switchTo)
        {
            var navButtons = GetComponentsInChildren<NavButton>();

            foreach (NavButton navButton in navButtons)
                if (navButton.TryGetComponent(out Button button))
                    button.onClick.AddListener(() => switchTo(navButton.targetMenuId));

            name = name.Replace("(Clone)", "");
        }

        /// <inheritdoc />
        public bool Is(string id)
        {
            return string.Equals(id, name, StringComparison.CurrentCultureIgnoreCase);
        }

        public void Open()
        {
            if (!ButtonsParent)
            {
                LogError($"ButtonsParent is null.");
                return;
            }
            ButtonsParent.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(defaultSelection);
            onOpen.Invoke();
        }

        public void Close()
        {
            if (!ButtonsParent)
            {
                LogError($"ButtonsParent is null.");
                return;
            }
            bool wasClosed = !ButtonsParent.gameObject.activeSelf;
            ButtonsParent.gameObject.SetActive(false);
            if (!wasClosed)
                onClose.Invoke();
        }

        protected override void Reset()
        {
            base.Reset();
            ButtonsParent = transform;
        }
    }
}