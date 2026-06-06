using System.Collections;
using UnityEngine;

/// <summary>
/// Clase base abstracta para personajes con vida (jugador, enemigos).
/// Gestiona salud, flash de daño, invencibilidad y el hook de knockback.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Personaje : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    [SerializeField] protected int maxHealth = 3;
    protected int currentHealth;

    protected DamageFlash damageFlash;

    // --- IDamageable ---
    public int  MaxHealth     => maxHealth;
    public int  CurrentHealth => currentHealth;
    public bool IsAlive       => currentHealth > 0;

    /// <summary>
    /// True mientras dure el flash tras recibir daño.
    /// Bloquea TakeDamage para evitar daño múltiple en el mismo instante.
    /// </summary>
    protected bool isInvincible;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        damageFlash   = GetComponentInChildren<DamageFlash>(includeInactive: true);

        if (damageFlash == null)
            Debug.LogWarning($"[{GetType().Name}] '{gameObject.name}' no tiene DamageFlash.");
    }

    /// <summary>
    /// Aplica daño. Ignorado si el personaje es invencible.
    /// sourcePosition se usa para calcular la dirección del knockback en subclases.
    /// </summary>
    public virtual void TakeDamage(int amount = 1, Vector2 sourcePosition = default)
    {
        if (!IsAlive || isInvincible) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida: {currentHealth}/{maxHealth}");

        OnDamaged();
        OnKnockback(sourcePosition); // hook de impulso físico (implementado en Player)

        if (!IsAlive)
            TriggerDeathSequence();
        else if (damageFlash != null)
        {
            isInvincible = true;
            StartCoroutine(InvincibilityFlash());
        }
    }

    /// <summary>
    /// Flash durante la invencibilidad. Al terminar desactiva isInvincible.
    /// </summary>
    private IEnumerator InvincibilityFlash()
    {
        yield return StartCoroutine(damageFlash.Flash());
        isInvincible = false;
    }

    /// <summary>
    /// Inicia la secuencia OnPreDeath → flash → Die().
    /// Accesible desde subclases (ej: Enemy.OnStomp).
    /// </summary>
    protected void TriggerDeathSequence()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(FlashThenDie());
    }

    private IEnumerator FlashThenDie()
    {
        OnPreDeath();

        if (damageFlash != null)
            yield return StartCoroutine(damageFlash.Flash());

        Die();
    }

    /// <summary>Hook de animación al recibir daño no fatal.</summary>
    protected virtual void OnDamaged() { }

    /// <summary>
    /// Hook de impulso físico al recibir daño.
    /// sourcePosition es la posición del objeto que causó el daño.
    /// Implementar en subclases que necesiten knockback (ej: Player).
    /// </summary>
    protected virtual void OnKnockback(Vector2 sourcePosition) { }

    /// <summary>Llamado antes del flash al morir: desactivar colliders y física.</summary>
    protected virtual void OnPreDeath() { }

    /// <summary>Comportamiento al morir. Se llama después del flash.</summary>
    protected abstract void Die();
}