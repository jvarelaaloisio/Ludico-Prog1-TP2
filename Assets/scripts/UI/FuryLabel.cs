using Core;
using TMPro;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using UnityEngine.UI;

namespace UI
{
    public class FuryLabel : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private TMP_Text label;
        //[AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private Image furyBarImage;
        [SerializeField] private string labelFormat = "Fury: {0}";
        [AutoMap(How.Service, When.Start)]
        private IFuryManager _furyManager;

        protected override void Start()
        {
            base.Start();

            if (_furyManager is not null)
            {
                _furyManager.OnFuryUpdated += HandleFuryUpdated;
                
                if (label)
                    label.SetText(string.Format(labelFormat, _furyManager.Fury));
                    
                if (furyBarImage)
                    furyBarImage.fillAmount = _furyManager.Fury;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        private void HandleFuryUpdated(float oldValue, float newValue)
        {
            if (label)
                label.SetText(string.Format(labelFormat, newValue));
            if (furyBarImage)
            furyBarImage.fillAmount = newValue;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_furyManager is not null)
                _furyManager.OnFuryUpdated -= HandleFuryUpdated;
        }
    }
}
