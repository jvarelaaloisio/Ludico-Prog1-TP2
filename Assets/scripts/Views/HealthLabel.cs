using System;
using HealthSystem;
using HealthSystem.Runtime;
using TMPro;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Core.Extensions;

namespace Views
{
    public class HealthLabel : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake, OnError.Ignore)]
        [SerializeField] private TMP_Text label;
        [SerializeField] private Ref<IHealthComponent> healthComponent;

        protected override async void OnEnable()
        {
            try
            {
                base.OnEnable();
                if (!healthComponent.HasValue)
                    return;
                Health health = null;
                while (health is null
                       && !DisableCancellationToken.IsCancellationRequested)
                {
                    health = healthComponent.Value.Health;
                    await Awaitable.NextFrameAsync();
                }
                if (DisableCancellationToken.IsCancellationRequested)
                    return;

                HandleOnDamage(health!.HP, health!.HP);
            }
            catch (Exception e) { LogException(e); }
        }

        public void HandleOnDamage(int before, int after)
            => label?.SetText(after.ToString().Colored(after > 0 ? Color.green : Color.red));
    }
}
