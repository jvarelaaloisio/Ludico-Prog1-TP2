using System.Collections.Generic;
using Core.Game;
using UnityEngine;
using UnityEngine.Events;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Components
{
    public class  EventOnGameState : MacacoBehaviour
    {
        [SerializeField] private List<GameState> targetStates;
        [SerializeField] private UnityEvent onEnterTargeted;
        [SerializeField] private UnityEvent onEnterOther;

        [AutoMap(How.Service, When.OnEnable)]
        private IGameManager _gameManager;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_gameManager is not null)
                _gameManager.OnStateChange += HandleStateChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_gameManager is not null)
                _gameManager.OnStateChange -= HandleStateChange;
        }

        private void HandleStateChange(GameState state)
        {
            if (targetStates.Contains(state))
                onEnterTargeted.Invoke();
            else
                onEnterOther.Invoke();
        }
    }
}
