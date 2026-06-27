using System;
using System.Collections.Generic;
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
        [Tooltip("Used to fake the menu ui while everything instantiates")]
        [SerializeField] private GameObject fakeMenu;
        [Tooltip("Used to hide all ui when a cinematic is showing")]
        [SerializeField] private CanvasGroup mainCanvasGroup;
        [SerializeField] private Ref<IMenu>[] menuPrefabs;
        [SerializeField] private string mainMenuId = "Menu_Main_View";
        [SerializeField] private string gameplayMenuId = "UI_Gameplay";
        [SerializeField] private string loseMenuId = "Menu_GameOver";
        [SerializeField] private string mainMenuLevelName = "MainMenu";
        [SerializeField] private string gameplayLevelName = "House";

        [AutoMap(How.Service, When.OnEnable)]
        private IGameManager _gameManager;

        private List<IMenu> _runtimeInstantiatedMenus;

        protected override void Reset()
        {
            base.Reset();
            menusParent = transform;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (fakeMenu)
                fakeMenu.SetActive(true);
            _runtimeInstantiatedMenus = new List<IMenu>(menuPrefabs.Length);
            foreach (var prefab in menuPrefabs)
            {
                if (!prefab.HasValue)
                    continue;
                IMenu menu = prefab.Instantiate(menusParent);
                menu.Setup(SwitchMenu);
                menu.gameObject.SetActive(menu.Is(mainMenuId));
                _runtimeInstantiatedMenus.Add(menu);
            }

            SwitchMenu(mainMenuId);
            if (fakeMenu)
                fakeMenu.SetActive(false);

            if (_gameManager is null)
                return;
            _gameManager.OnEnterLevel += HandleEnterLevel;
            _gameManager.OnLose += HandleLose;
            _gameManager.OnStateChange += HandleStateChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_gameManager is null)
                return;
            _gameManager.OnEnterLevel -= HandleEnterLevel;
            _gameManager.OnLose -= HandleLose;
            _gameManager.OnStateChange -= HandleStateChange;
        }

        private void HandleEnterLevel(string levelName)
        {
            if (string.Equals(levelName, mainMenuLevelName, StringComparison.CurrentCultureIgnoreCase))
                SwitchMenu(mainMenuId);
        }

        private void HandleLose()
            => SwitchMenu(loseMenuId);

        private void HandleStateChange(GameState state)
        {
            //TODO: Change for a switch
            bool isCinematic = state is GameState.Day1 or GameState.Day2;
            mainCanvasGroup.alpha = isCinematic ? 0 : 1;
            mainCanvasGroup.interactable = !isCinematic;
            mainCanvasGroup.blocksRaycasts = !isCinematic;
            Log("Turning menu " + (isCinematic ? "Off" : "On"));
            bool isGameplay = state is GameState.Night1 or GameState.Night2;
            if (isGameplay)
                SwitchMenu(gameplayMenuId);
        }

        private void SwitchMenu(string id)
        {
            foreach (IMenu menu in _runtimeInstantiatedMenus)
            {
                if (menu.Is(id))
                    menu.Open();
                else
                    menu.Close();
            }

            Debug.Log("Changing to menu: " + id);
        }
    }
}
