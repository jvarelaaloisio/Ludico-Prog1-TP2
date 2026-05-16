using System.Threading;
using VarelaAloisio.Core.Utils;

namespace VarelaAloisio.Core
{
    public class MonoBehaviourAsync : MacacoBehaviour
    {
        private CancellationTokenSource _disableCancellationTokenSource;

        /// <summary> Token cancelled when the component is disabled.
        /// <p> When the component is disabled, this returns a new canceled token. </p>
        /// <p> If you need, for some strange and remote reason, direct access to the token source, call <see cref="LinkWithDisable"/> and fill the target parameter with null. THIS IS NOT RECOMMENDED THOUGH.</p>
        /// </summary>
        /// <remarks> This token is re-created when the component is re-enabled. </remarks>
        public CancellationToken DisableCancellationToken => _disableCancellationTokenSource?.Token ?? new CancellationToken(true);

        protected override void OnEnable()
        {
            _disableCancellationTokenSource = new CancellationTokenSource();
            base.OnEnable();
        }

        protected virtual void OnDisable()
            => TokenUtils.CancelAndDispose(ref _disableCancellationTokenSource);

        /// <param name="target">If null, this method directly returns <see cref="_disableCancellationTokenSource"/></param>
        /// <returns> A linked token source, combining the given token with the <see cref="_disableCancellationTokenSource"/></returns>
        protected CancellationToken LinkWithDisable(CancellationToken? target)
            => target.HasValue
                   ? CancellationTokenSource.CreateLinkedTokenSource(DisableCancellationToken, target.Value).Token
                   : DisableCancellationToken;
    }
}