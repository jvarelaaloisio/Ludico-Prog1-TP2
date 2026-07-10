using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using VarelaAloisio.Core;

namespace Localization
{
    public class LanguageSelector : MacacoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        private List<Locale> _locales = new ();

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            dropdown.onValueChanged.AddListener(HandleDropdownChanged);
        }

        private void HandleDropdownChanged(int index)
            => SelectLocale(_locales[index]);

        protected override void Start()
        {
            base.Start();
                LocalizationSettings.InitializationOperation.Completed += HandleLocalizationInitialized;
                return;
            if (LocalizationSettings.PreloadBehavior is not PreloadBehavior.PreloadAllLocales
                && !LocalizationSettings.InitializationOperation.IsDone)
            {
                return;
            }

            SetupLocales(LocalizationSettings.Instance);
        }

        private void SelectLocale(Locale locale)
        {
            LocalizationSettings.Instance.SetSelectedLocale(locale);
            SetupLocales(LocalizationSettings.Instance);
        }

        private void HandleLocalizationInitialized(AsyncOperationHandle<LocalizationSettings> handle)
            => SetupLocales(handle.Result);

        private void SetupLocales(LocalizationSettings settings)
        {
            if (_locales.Count == settings.GetAvailableLocales().Locales.Count)
                return;
            dropdown.gameObject.SetActive(true);
            dropdown.ClearOptions();
            _locales = settings.GetAvailableLocales().Locales;
            foreach (Locale locale in settings.GetAvailableLocales().Locales)
                dropdown.options.Add(new TMP_Dropdown.OptionData(locale.Identifier.CultureInfo.NativeName));
            dropdown.SetValueWithoutNotify(_locales.IndexOf(settings.GetSelectedLocale()));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LocalizationSettings.InitializationOperation.Completed -= HandleLocalizationInitialized;
        }
    }
}
