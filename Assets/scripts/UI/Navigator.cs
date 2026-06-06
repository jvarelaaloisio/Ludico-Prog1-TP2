using System;
using Core;
using Core.UI;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI
{
    public class Navigator : MacacoBehaviour
    {
        [SerializeField] private Transform menusParent;
        [SerializeField] private Ref<IMenu>[] menus;
        [SerializeField] private string mainMenuId = "Menu_Main_View";
        [SerializeField] private string gameplayMenuId = "UI_Gameplay";
        [SerializeField] private string gameplayLevelName = "House";
        [AutoMap(How.Service, When.OnEnable)]
        private IGameManager _gameManager;


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
            _gameManager.OnEnterLevel += HandleEnterLevel;
        }

        private void HandleEnterLevel(string levelName)
        {
            if (string.Equals(levelName, gameplayLevelName, StringComparison.CurrentCultureIgnoreCase))
                SwitchMenu(gameplayMenuId);
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
