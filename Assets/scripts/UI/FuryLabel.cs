using Core;
using TMPro;
using UnityEngine;
using System.Collections;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UI
{
    public class FuryLabel : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image furyBarImage;
        [SerializeField] private string labelFormat = "Fury: {0}";
        [AutoMap(How.Service, When.Start)]
        private IFuryManager _furyManager;
        [SerializeField] private Volume _globalVolume;
        private Vignette _vignette;
        private LensDistortion _lensDistortion;
        private ChromaticAberration _aberration;
        [SerializeField] private float maxVignetteIntensity = 0.8f;
        [SerializeField] private float maxLensDistortion = -0.7f;
        [SerializeField] private float maxChromaticAberration = 0.5f;


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
            if (_globalVolume)
            {
                _globalVolume.profile.TryGet(out _vignette);
                _globalVolume.profile.TryGet(out _lensDistortion);
                _globalVolume.profile.TryGet(out _aberration);
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
            if (newValue > oldValue)
            {
               if (_vignette != null && _lensDistortion != null && _aberration != null)
                {
                    _vignette.intensity.value = Mathf.Clamp(maxVignetteIntensity * newValue, 0f, maxVignetteIntensity);
                    _lensDistortion.intensity.value = Mathf.Clamp(maxLensDistortion * newValue, maxLensDistortion, 0f);
                    _aberration.intensity.value = Mathf.Clamp(maxChromaticAberration * newValue, 0f, maxChromaticAberration);
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_furyManager is not null)
                _furyManager.OnFuryUpdated -= HandleFuryUpdated;
        }
    }
}
