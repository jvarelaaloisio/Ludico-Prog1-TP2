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

        // [AutoMap(How.Service, When.Start)]
        // private EventSystem _eventSystem;

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

            SwitchMenu("Menu_Main_View");
        }

        private void SwitchMenu(string id)
        {
            // recorro menues
            foreach (var menu in menus)
            {
                if (!menu.HasValue) continue;

                // comparo si el nombre coincide con el id
                bool isTargetMenu = menu.Value.ButtonsParent.gameObject.name == id;

                // prende si coincide, apaga si no
                menu.Value.ButtonsParent.gameObject.SetActive(isTargetMenu);
            }

            Debug.Log("Intentando cambiar al menú: " + id);
        }
    }
}
