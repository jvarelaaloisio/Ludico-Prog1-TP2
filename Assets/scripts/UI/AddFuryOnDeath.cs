using System;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using Core;
using Core.Game;
using HealthSystem.Runtime.Components;


public class AddFuryOnDeath : MacacoBehaviour
{
    [SerializeField] private float furyToAdd = 0.1f;
    [AutoMap(How.Service, When.Start)]
    private IFuryManager _furyManager;
    private HealthComponentExtended _healthComponent;

    protected override void Start()
    {
        base.Start();
        _healthComponent = GetComponent<HealthComponentExtended>();
        if (_healthComponent != null)
        {
            _healthComponent.onDeath.AddListener(OnDeath);
        }
    }

    private void OnDeath()
    {
        if (_furyManager != null)
        {
            _furyManager.AddFury(furyToAdd);
        }
    }
    protected override void OnDisable()          
    {
        base.OnDisable();
        if (_healthComponent != null)
        {
            _healthComponent.onDeath.RemoveListener(OnDeath);
        }
    }

}
