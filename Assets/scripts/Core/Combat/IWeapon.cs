using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Combat
{
    public interface IWeapon
    {
        Task Attack(CancellationToken token);
        void SetOwner(Transform newOwner);
        void Release();
        bool IsOnCooldown { get; }
    }
}