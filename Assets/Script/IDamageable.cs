using UnityEngine;

/// <summary>
/// Entidades que pueden recibir daño y tienen puntos de vida.
/// sourcePosition permite a los receptores calcular la dirección del knockback.
/// </summary>
public interface IDamageable
{
    int  MaxHealth     { get; }
    int  CurrentHealth { get; }
    bool IsAlive       { get; }
    void TakeDamage(int amount = 1, Vector2 sourcePosition = default);
}