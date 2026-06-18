using System.Threading;
using System.Threading.Tasks;

namespace Core.Combat
{
    /// <summary /> A utility to be able to charge damage from 0 to a max value
    public interface ICharger
    {
        /// <summary /> Current charge state. Range: [0..<see cref="MaxCharge"/>]
        float Charge { get; }

        /// <summary /> The minimum value possible for the charge
        float MinCharge { get; }
        /// <summary /> The maximum value possible for the charge
        float MaxCharge { get; }
        /// <summary /> If this is charging.
        /// <remarks>Start: <see cref="StartCharging"/> and stop using it's given token</remarks>
        bool IsCharging { get; }

        /// <summary /> Reset <see cref="Charge"/>
        /// <remarks>Doesn't stop charging</remarks>
        void ResetCharge();
        /// <summary /> Starts charging over time. You can access current value through <see cref="Charge"/>
        /// <param name="token">Use this to stop charging</param>
        Task StartCharging(CancellationToken token);
    }
}