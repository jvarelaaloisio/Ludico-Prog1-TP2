using System;
using System.Threading;
using Core.Game;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Components
{
    public class FollowCharacter : MacacoBehaviour
    {
        [SerializeField] private string characterId;
        [SerializeField] private Ref<ICharacter> target;
        [SerializeField] private Vector3 offset;
        [Tooltip("If true, the component will fetch the target, even if a value is already set from inspector")]
        [SerializeField] private bool ignoreInspectorSetCharacter = false;
        [AutoMap(How.Service, When.OnEnable, OnError.Ignore)]
        private ICharacterRepository _characterRepository;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!target.HasValue || ignoreInspectorSetCharacter)
                FetchTarget(DisableCancellationToken);
        }

        private void LateUpdate()
        {
            if (!target.HasValue)
                return;
            transform.position = target.Value.transform.position + offset;
        }

        private async void FetchTarget(CancellationToken token)
        {
            try
            {
                Log($"Seeking target with id: {characterId}");
                while (!token.IsCancellationRequested)
                {
                    if (_characterRepository.TryGet(characterId, out ICharacter character))
                    {
                        target.Value = character;
                        break;
                    }
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (Exception e) { LogException(e); }
        }
    }
}