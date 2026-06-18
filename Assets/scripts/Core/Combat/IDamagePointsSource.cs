using System;

namespace Core.Combat
{
    /// <summary /> Returns how much damage to do.
    public interface IDamagePointsSource
    {
        /// <summary /> Function to add a multiplier to the attack. Not necessary for normal functionality.
        Func<float, float> DamageMultiplier { set; } 
        /// <summary /> How much damage to do.
        /// <remarks>Can change over time.</remarks>
        int RoundedDamage { get; }
        /// <summary /> Same as <see cref="RoundedDamage"/> but in float form.
        /// <remarks>May have decimal accuracy</remarks>
        float Damage { get; }
    }
}