using Core.Audio;
using UnityEngine;
using UnityEngine.UI;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace UI.Buttons
{
    public class VolumeSlider : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Awake)]
        [SerializeField] private Slider slider;
        [SerializeField] private VolumeChannel channel;
        [AutoMap(How.Service, When.OnEnable)]
        private IAudioManager _audioManager;
        protected override void Reset()
        {
            base.Reset();
            slider = GetComponent<Slider>();
            if (!slider)
                slider = GetComponentInChildren<Slider>();
            if (!slider)
                return;

            slider.minValue = -1;
            slider.maxValue = 1;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!slider)
                return;
            slider.onValueChanged.AddListener(HandleSliderInput);
            if (_audioManager?.TryGetVolume(channel, out float volume) ?? false)
                slider.SetValueWithoutNotify(volume);
            else
                slider.interactable = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (slider)
                slider.onValueChanged.RemoveListener(HandleSliderInput);
        }

        private void HandleSliderInput(float value)
        {
            _audioManager?.SetVolume(channel, value);
        }
    }
}