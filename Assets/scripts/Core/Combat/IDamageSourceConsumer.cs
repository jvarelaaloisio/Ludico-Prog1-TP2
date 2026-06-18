namespace Core.Combat
{
    public interface IDamageSourceConsumer
    {
        IDamagePointsSource DamageSource { set; }
    }
}