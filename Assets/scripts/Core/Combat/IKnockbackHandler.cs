using UnityEngine;

namespace Core.Combat
{
    public interface IKnockbackHandler
    {
        void Handle(Vector2 direction, float forceMultiplier);
    }
}