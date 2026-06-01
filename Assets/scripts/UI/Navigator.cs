using System;
using Core.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI
{
    public class Navigator : MacacoBehaviour
    {
        [SerializeField] private Transform menusParent;
        [SerializeField] private Ref<IMenu>[] menus;
        [SerializeField] private string mainMenuId = "Menu_Main_View";

        protected override void Reset()
        {
            base.Reset();
            menusParent = transform;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            foreach (var menu in menus)
            {
                if (!menu.HasValue)
                    continue;
                menu.Value.Configuration.Setup(menu.Value.ButtonsParent, SwitchMenu);
            }

            SwitchMenu(mainMenuId);
        }

        private void SwitchMenu(string id)
        {
            foreach (var menu in menus)
            {
                if (!menu.HasValue) continue;
                
                bool isTargetMenu = menu.Value.ButtonsParent.gameObject.name == id;
                
                menu.Value.ButtonsParent.gameObject.SetActive(isTargetMenu);
            }

            Debug.Log("Changing to menu: " + id);
        }
    }
}
