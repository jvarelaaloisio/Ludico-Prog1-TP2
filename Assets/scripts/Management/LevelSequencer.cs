using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Combat;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;

namespace Management
{
    public class LevelSequencer : MacacoBehaviour, ILevelSequencer
    {
        [SerializeField] private Ref<ICharacter> peacefulCharacter;
        [SerializeField] private Ref<IController> peacefulCharacterController;
        [SerializeField] private Ref<IWeapon> peacefulCharacterWeapon;
        [SerializeField] private Transform peacefulCharacterSpawnPoint;
        [SerializeField] private float furyThresholdToSpawnPeacefulCharacter = .89f;
        [SerializeField] private float furyThresholdToEndLevel = .99f;
        [Header("End level sequence")]
        [SerializeField] private Ref<IProgress<float>> flashEffect;
        [SerializeField] private AnimationCurve flashEffectCurve = AnimationCurve.EaseInOut(0, 0, 2, 1);
        [SerializeField] private float holdFlashDuration = 3;

        private IFuryManager _furyManager;

        protected override void Start()
        {
            base.Start();
            SetupLevel();
        }

        public void SetupLevel()
        {
            if (!Service.TryGet(out _furyManager))
            {
                LogError($"Fury Manager not found in service.");
                return;
            }
            _furyManager.OnFuryUpdated += HandleFuryUpdated;
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Service.TryGet(out _furyManager))
                _furyManager.OnFuryUpdated -= HandleFuryUpdated;
        }

        private void HandleFuryUpdated(float before, float after)
        {
            if (after < furyThresholdToSpawnPeacefulCharacter)
                return;
            if (after < furyThresholdToEndLevel)
                _ = SpawnPeacefulCreature(DisableCancellationToken);
            else
                _ = EndLevel(DisableCancellationToken);

        }

        private async Task SpawnPeacefulCreature(CancellationToken token)
        {
            try
            {
                if (!peacefulCharacter.HasValue)
                {
                    LogError($"Peaceful character prefab is null");
                    return;
                }
                ICharacter character = await peacefulCharacter.InstantiateAsync();
                if (token.IsCancellationRequested)
                    return;
                character.transform.position = peacefulCharacterSpawnPoint.position;
                if (!peacefulCharacterController.HasValue)
                {
                    LogError($"Peaceful character controller prefab is null");
                    return;
                }
                IController controller = await peacefulCharacterController.InstantiateAsync();
                if (token.IsCancellationRequested)
                    return;
                controller.Setup(character);
                if (!peacefulCharacterWeapon.HasValue)
                {
                    LogWarning($"Peaceful character weapon prefab is null");
                    return;
                }
                IWeapon weapon = await peacefulCharacterWeapon.InstantiateAsync();
                if (token.IsCancellationRequested)
                    return;
                weapon.transform.position = peacefulCharacterSpawnPoint.position;
            }
            catch (Exception e) { LogException(e); }
        }

        private async Task EndLevel(CancellationToken token)
        {
            _furyManager.ResetFury();
            if (!flashEffect.HasValue)
            {
                LogError($"Flash effect is null.");
                return;
            }

            float start = Time.time;
            float now = 0;
            float duration = flashEffectCurve.keys[^1].time - flashEffectCurve.keys[0].time;
            do
            {
                now = Time.time;
                float lerp = (now - start) / duration;
                float slerp = flashEffectCurve.Evaluate(lerp);
                flashEffect.Value.Report(slerp);
                await Awaitable.NextFrameAsync();
            } while (now < start + duration
                     && !token.IsCancellationRequested);

            if (token.IsCancellationRequested)
                return;
            if (Service.TryGet(out IGameManager gameManager))
                await gameManager.WinLevel(holdFlashDuration);
            else
                LogError($"Game Manager not found in service.");
        }
    }
}