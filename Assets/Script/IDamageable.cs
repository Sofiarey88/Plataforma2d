using System;
using UnityEngine;

public interface IDamageable
{
    int MaxHealth { get; }
    int CurrentHealth { get; }
    bool IsAlive { get; }
    void TakeDamage(int amount = 1, Vector2 sourcePosition = default);

    event Action<int, int> OnHealthChanged; 
}