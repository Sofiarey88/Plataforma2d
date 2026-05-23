/// <summary>
/// Entidades que pueden recibir daño y tienen puntos de vida.
/// </summary>
public interface IDamageable
{
    int MaxHealth { get; }
    int CurrentHealth { get; }
    bool IsAlive { get; }
    void TakeDamage(int amount = 1);
}