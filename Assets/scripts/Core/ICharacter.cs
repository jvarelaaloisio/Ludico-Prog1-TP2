using Core.Combat;

namespace Core
{
    public interface ICharacter
    {
        void PickUp(IWeapon weapon);
        bool TryAttack();
        IWeapon CurrentWeapon { get; }
    }
}