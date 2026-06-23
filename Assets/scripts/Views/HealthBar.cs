using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game;
using HealthSystem;
using UnityEngine;
using UnityEngine.UI;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Views
{
    public class HealthBar : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake)]
        [SerializeField] private Image image;
        [SerializeField] private string targetId = "Player";
        [SerializeField] private float animationSpeed = 1;

        [AutoMap(How.Service, When.OnEnable)]
        private ICharacterRepository _characterRepository;
        private int _maxHp;
        private float _targetValue;
        private float _modificationSign;

        protected override void OnEnable()
        {
            base.OnEnable();
            _ = FetchTarget(DisableCancellationToken);
        }

        private async Task FetchTarget(CancellationToken token)
        {
            try
            {
                Log($"Seeking target with id: {targetId}");
                while (!token.IsCancellationRequested)
                {
                    if (_characterRepository.TryGet(targetId, out ICharacter character))
                    {
                        Health health = character.HealthComponent.Health;
                        _maxHp = health.MaxHP;
                        health.OnDamage += HandleHpChanged;
                        health.OnHeal += HandleHpChanged;
                        image.fillAmount = (float)health.HP / _maxHp;
                        HandleHpChanged(0, health.HP);
                        break;
                    }
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (Exception e) { LogException(e); }
        }

        private void Update()
        {
            if (image
                && (_modificationSign > 0 && image.fillAmount < _targetValue
                    || (_modificationSign < 0 && image.fillAmount > _targetValue)))
            {
                image.fillAmount += Time.deltaTime * _modificationSign * animationSpeed;
                if ((_modificationSign > 0 && image.fillAmount >= _targetValue
                     || (_modificationSign < 0 && image.fillAmount <= _targetValue)))
                    image.fillAmount = _targetValue;
            }
            bool IsApproximately(float a, float b)
                => Mathf.Abs(a - b) <= 0.025f;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_characterRepository.TryGet(targetId, out ICharacter character))
            {
                Health health = character.HealthComponent.Health;
                health.OnDamage += HandleHpChanged;
                health.OnHeal += HandleHpChanged;
            }
        }

        private void HandleHpChanged(int before, int after)
        {
            _targetValue = (float)after / _maxHp;
            _modificationSign = Mathf.Sign(after - before);
        }
    }
}