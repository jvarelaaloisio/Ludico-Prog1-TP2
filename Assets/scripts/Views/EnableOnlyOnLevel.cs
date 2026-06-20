using System;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Views
{
    public class EnableOnlyOnLevel : MacacoBehaviour
    {
        [SerializeField] private string levelName = "House";
        [AutoMap(How.Service, When.Awake)]
        private IGameManager _gameManager;

        protected override void Awake()
        {
            base.Awake();
            if (_gameManager is not null)
                _gameManager.OnEnterLevel += HandleEnterLevel;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_gameManager is not null)
                _gameManager.OnEnterLevel -= HandleEnterLevel;
        }

        private void HandleEnterLevel(string levelName)
        {
            gameObject.SetActive(string.Equals(levelName, this.levelName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
