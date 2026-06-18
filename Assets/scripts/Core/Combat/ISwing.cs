using System;
using UnityEngine;

namespace Core.Combat
{
    public interface ISwing
    {
        /// <summary /> Called when the swing starts
        event Action OnSwing;
        /// <summary /> Called when the swing ends
        event Action OnSwung;
        /// <summary /> Calculate the position for a given point in the rotation.
        /// <param name="lerp">A [0..1] range representing the state of the rotation.
        /// <para>0 is the start of the rotation, 1 is the end of it</para> </param>
        Vector3 CalculatePosition(float lerp);

        /// <summary /> Calculate the direction for a given point in the rotation.
        /// <param name="lerp">A [0..1] range representing the state of the rotation.
        /// <para>0 is the start of the rotation, 1 is the end of it</para> </param>
        Vector3 CalculateDirection(float lerp);

        /// <summary>
        /// Set the position and direction of the sprite. Use <see cref="MeleeWeapon.CalculatePosition"/> and <see cref="MeleeWeapon.CalculateDirection"/> to get the values you need.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        void SetPositionAndDirection(Vector3 position, Vector3 direction);

        void CacheSwingPositionAndRotation(out Vector3 originalPosition, out Quaternion originalRotation);
        void SetPositionAndRotation(Vector3 originalPosition, Quaternion originalRotation);
    }
}