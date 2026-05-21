using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using VarelaAloisio.Core;

namespace Combat
{
    public class Weapon : MonoBehaviourAsync
    {
        [SerializeField] private Collider2D damageTrigger;
        [SerializeField] private float scale = 1;
        [SerializeField] private float duration = .25f;
        [SerializeField] private float rotationOffset;
        [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField] private Transform testPivot;
        [ContextMenu("Swing")]
        private void DoTestRotation()
            => Swing(testPivot, DisableCancellationToken);

        private void Reset()
        {
            damageTrigger = GetComponent<Collider2D>();
            damageTrigger ??= gameObject.AddComponent<BoxCollider2D>();
            damageTrigger.isTrigger = true;
        }

        public async Task Swing(Transform pivot, CancellationToken token)
        {
            Vector3 originalPosition = transform.localPosition;
            Quaternion originalRotation = transform.localRotation;
            damageTrigger.enabled = true;
            CancellationTokenRegistration registration = token.Register(CleanUp);
            const float pi = Mathf.PI;
            float now = Time.time;
            float start = Time.time;
            while (now - start < duration)
            {
                now = Time.time;
                float lerp = swingCurve.Evaluate((now - start) / duration);
                float x = (lerp + rotationOffset) * pi;
                Vector3 position = pivot.position + new Vector3(Mathf.Cos(x) * scale, Mathf.Sin(x) * scale);
                Vector3 direction = new Vector3(-Mathf.Sin(x), Mathf.Cos(x));
                Debug.DrawRay(position, direction, Color.red, 1);
                transform.SetLocalPositionAndRotation(position, Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, -90));
                await Awaitable.NextFrameAsync();
                if (token.IsCancellationRequested)
                    return;
            }
            CleanUp();
            await registration.DisposeAsync();
            return;

            void CleanUp()
            {
                damageTrigger.enabled = false;
                transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
            }
        }
    }
}
