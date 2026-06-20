using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Game;
using HealthSystem.Runtime.Components;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace AI
{
    public class Pig : MacacoBehaviour, IController
    {
        [SerializeField] private Ref<ICharacter> character;
        [SerializeField] private string playerId = "Player";
        [SerializeField] private Ref<ICharacter> player;
        [AutoMap(How.Service, When.OnEnable, OnError.Ignore)]
        private ICharacterRepository _characterRepository;

        [Header("Setup")]
        [SerializeField] private float awarenessDistance = 3;

        [Space]
        [SerializeField] private bool selfSetup;

        private HealthComponent _health;
        public ICharacter Character => character.HasValue ? character.Value : null;

        public void Setup(ICharacter injectedCharacter)
        {
            character.Value = injectedCharacter;
            FetchPlayer();
            _health = character.Value.gameObject.GetComponent<HealthComponent>()
                      ?? character.Value.gameObject.GetComponentInChildren<HealthComponent>();

            if (_health is null)
                return;
            _health.Setup();
            _health.Health.OnDeath += HandleDeath;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!selfSetup)
                return;
            if (!character.HasValue)
            {
                LogError("Character not set");
                return;
            }
            Setup(character.Value);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!character.HasValue)
                return;
            _health = character.Value.gameObject.GetComponent<HealthComponent>()
                      ?? character.Value.gameObject.GetComponentInChildren<HealthComponent>();

            if (_health is null)
                return;
            _health.Setup();
            _health.Health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            if (character.HasValue)
                character.Value.gameObject.SetActive(false);
            enabled = false;
        }

        private async void FetchPlayer()
        {
            try
            {
                while (!player.HasValue
                       && !DisableCancellationToken.IsCancellationRequested)
                {
                    if (_characterRepository.TryGet(playerId, out ICharacter playerCharacter))
                    {
                        player.Value = playerCharacter;
                        break;
                    }
                    await Awaitable.NextFrameAsync();
                }

                if (!DisableCancellationToken.IsCancellationRequested)
                    _ = WaitForPlayerToComeClose();
            }
            catch (Exception e) { LogException(e); }
        }

        private async Task WaitForPlayerToComeClose()
        {
            try
            {
                while (player.HasValue
                       && !DisableCancellationToken.IsCancellationRequested)
                {
                    if (Vector3.Distance(player.Value.transform.position, character.Value.transform.position) <= awarenessDistance)
                    {
                        _ = FleeFromPlayer();
                        return;
                    }

                    await Awaitable.NextFrameAsync();
                }
            }
            catch (Exception e) { LogException(e); }
        }

        private async Task FleeFromPlayer()
        {
            try
            {
                using var moveTokenSource = new CancellationTokenSource();
                character.Value.Move(LinkWithDisable(moveTokenSource.Token));
                while (player.HasValue
                       && !DisableCancellationToken.IsCancellationRequested)
                {
                    character.Value.Direction = (character.Value.transform.position - player.Value.transform.position).normalized;
                    await Awaitable.NextFrameAsync();
                }
                moveTokenSource.Cancel();
            }
            catch (Exception e) { LogException(e); }
        }

        private void OnDrawGizmosSelected()
        {
            if (!character.HasValue)
                return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(character.Value.transform.position, awarenessDistance);
        }
    }
}