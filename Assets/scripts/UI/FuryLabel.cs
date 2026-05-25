using Core;
using TMPro;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI
{
    public class FuryLabel : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private TMP_Text label;
        [SerializeField] private string labelFormat = "Fury: {0}";
        [AutoMap(How.Service, When.OnEnable)]
        private IFuryManager _furyManager;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_furyManager is not null)
                _furyManager.OnFuryUpdated += HandleFuryUpdated;
            if (label)
                label.SetText(string.Format(labelFormat, _furyManager?.Fury));
        }

        private void HandleFuryUpdated(float oldValue, float newValue)
        {
            if (label)
                label.SetText(string.Format(labelFormat, newValue));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_furyManager is not null)
                _furyManager.OnFuryUpdated -= HandleFuryUpdated;
        }
    }
}
