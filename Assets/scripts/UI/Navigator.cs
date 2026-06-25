using System;
using Core.Game;
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
        [SerializeField] private string loseMenuId = "Menu_GameOver";
        [SerializeField] private string mainMenuLevelName = "MainMenu";
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

            _gameManager.OnEnterLevel += HandleEnterLevel;
            _gameManager.OnLose += HandleLose;
            _gameManager.OnStateChange += HandleStateChange;
        }

        protected override void Start()
        {
            base.Start();
            SwitchMenu(mainMenuId);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _gameManager.OnEnterLevel -= HandleEnterLevel;
            _gameManager.OnLose -= HandleLose;
            _gameManager.OnStateChange -= HandleStateChange;
        }

        private void HandleEnterLevel(string levelName)
        {
            if (string.Equals(levelName, gameplayLevelName, StringComparison.CurrentCultureIgnoreCase))
                SwitchMenu(gameplayMenuId);
            if (string.Equals(levelName, mainMenuLevelName, StringComparison.CurrentCultureIgnoreCase))
                SwitchMenu(mainMenuId);
        }

        private void HandleLose()
            => SwitchMenu(loseMenuId);

        private void HandleStateChange(GameState state)
        {
            bool isCinematic = state is GameState.Day1 or GameState.Day2;
            menusParent.gameObject.SetActive(!isCinematic);
            Log("Turning menu " + (isCinematic ? "Off" : "On"));
        }

        private void SwitchMenu(string id)
        {
            foreach (var menu in menus)
            {
                if (!menu.HasValue) continue;

                bool isTargetMenu = menu.Value.ButtonsParent.gameObject.name == id;

                if (isTargetMenu)
                    menu.Value.Open();
                else
                    menu.Value.Close();
            }

            Debug.Log("Changing to menu: " + id);
        }
    }
}
