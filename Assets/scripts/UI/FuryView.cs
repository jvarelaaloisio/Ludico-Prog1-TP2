using Core;
using TMPro;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UI
{
    public class FuryView : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image furyBarImage;
        [SerializeField] private string labelFormat = "Fury: {0}";
        [SerializeField] private Volume _globalVolume;
        [SerializeField] private float maxVignetteIntensity = 0.8f;
        [SerializeField] private float maxLensDistortion = -0.7f;
        [SerializeField] private float maxChromaticAberration = 0.5f;
        [SerializeField] private float valueAnimationSpeed = 10f;
        [SerializeField] private float aberrationFrequency = 10f;

        [AutoMap(How.Service, When.Start)]
        private IFuryManager _furyManager;
        private Vignette _vignette;
        private LensDistortion _lensDistortion;
        private ChromaticAberration _aberration;
        private float _furyBarTargetFill;
        private float _vignetteTargetValue;
        private float _distortionTargetValue;
        private float _aberrationTargetValue;
        private float _furyModificationSign;


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

        private void Update()
        {
            if (furyBarImage
                && !IsApproximately(_furyBarTargetFill, furyBarImage.fillAmount))
            {
                furyBarImage.fillAmount += Time.deltaTime * _furyModificationSign * valueAnimationSpeed;
                if (IsApproximately(_furyBarTargetFill, furyBarImage.fillAmount))
                    furyBarImage.fillAmount = _furyBarTargetFill;
            }
            if (_vignette
                && !IsApproximately(_vignetteTargetValue, _vignette.intensity.value))
            {
                _vignette.intensity.value += Time.deltaTime * _furyModificationSign * valueAnimationSpeed;
                if (IsApproximately(_vignetteTargetValue, _vignette.intensity.value))
                    _vignette.intensity.value = _vignetteTargetValue;
            }
            if (_lensDistortion
                && !IsApproximately(_distortionTargetValue, _lensDistortion.intensity.value))
            {
                _lensDistortion.intensity.value -= Time.deltaTime * _furyModificationSign * valueAnimationSpeed;
                if (IsApproximately(_distortionTargetValue, _lensDistortion.intensity.value))
                    _lensDistortion.intensity.value = _distortionTargetValue;
            }
            if (_aberration)
                _aberration.intensity.value = Mathf.Sin(Time.time * aberrationFrequency) * _aberrationTargetValue;

            return;

            bool IsApproximately(float a, float b)
                => Mathf.Abs(a - b) <= 0.05f;
        }

        private void HandleFuryUpdated(float oldValue, float newValue)
        {
            if (label)
                label.SetText(string.Format(labelFormat, newValue));
            _furyBarTargetFill = newValue;
            _vignetteTargetValue = Mathf.Clamp(maxVignetteIntensity * newValue, 0f, maxVignetteIntensity);
            _distortionTargetValue = Mathf.Clamp(maxLensDistortion * newValue, maxLensDistortion, 0f);
            _aberrationTargetValue = Mathf.Clamp(maxChromaticAberration * newValue, 0f, maxChromaticAberration);
            _furyModificationSign = Mathf.Sign(newValue - oldValue);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_furyManager is not null)
                _furyManager.OnFuryUpdated -= HandleFuryUpdated;
        }
    }
}
