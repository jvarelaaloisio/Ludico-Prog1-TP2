using System;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using Core;
using UnityEngine.SceneManagement;

public class GameOverHandler : MacacoBehaviour
{ 
    [AutoMap(How.Service, When.Start)]
    private IGameManager _gameManager;
    [AutoMap(How.Service, When.Start)]
    private IFuryManager _furyManager;

    public void RestartGame(){
        if (_furyManager != null)
        {
            _furyManager.ResetFury();
        }
        if (_gameManager != null)
        {
        _gameManager.EnterGame();
        }
    }
}
