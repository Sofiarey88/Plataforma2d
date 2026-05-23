using UnityEngine;

/// <summary>
/// Clase base abstracta para personajes con vida (jugador, enemigos).
/// Implementa IDamageable con lógica de salud compartida.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Personaje : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    [SerializeField] protected int maxHealth = 3;
    protected int currentHealth;

    // --- IDamageable ---
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Aplica daño al personaje. Llama a Die() si la vida llega a 0.
    /// </summary>
    public virtual void TakeDamage(int amount = 1)
    {
        if (!IsAlive) return;
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}/{maxHealth}");
        OnDamaged();
        if (!IsAlive) Die();
    }

    /// <summary>
    /// Hook llamado justo después de recibir daño (para animaciones, efectos, etc.).
    /// </summary>
    protected virtual void OnDamaged() { }

    /// <summary>
    /// Comportamiento al morir. Obligatorio en clases hijas.
    /// </summary>
    protected abstract void Die();
}