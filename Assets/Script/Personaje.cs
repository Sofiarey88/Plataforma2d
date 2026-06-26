using System.Collections;
using UnityEngine;

// Clase base abstracta para personajes con vida (jugador, enemigos).
// Gestiona salud, flash de daño, invencibilidad y el hook de knockback.
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Personaje : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    [SerializeField] protected int maxHealth = 3;
    protected int currentHealth;

    public event System.Action<int, int> OnHealthChanged;


    protected DamageFlash damageFlash;

    // --- IDamageable ---
    public int  MaxHealth     => maxHealth;
    public int  CurrentHealth => currentHealth;
    public bool IsAlive       => currentHealth > 0;

    protected bool isInvincible;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        damageFlash   = GetComponentInChildren<DamageFlash>(includeInactive: true);

        if (damageFlash == null)
            Debug.LogWarning($"[{GetType().Name}] '{gameObject.name}' no tiene DamageFlash.");
    }

    // Aplica daño. Ignorado si el personaje es invencible.
    // sourcePosition se usa para calcular la dirección del knockback en subclases.
    public virtual void TakeDamage(int amount = 1, Vector2 sourcePosition = default)
    {
        if (!IsAlive || isInvincible) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // ← mensaje emitido

        OnDamaged();
        OnKnockback(sourcePosition);

        if (!IsAlive)
            TriggerDeathSequence();
        else if (damageFlash != null)
        {
            isInvincible = true;
            StartCoroutine(InvincibilityFlash());
        }
    }

    // Flash durante la invencibilidad. Al terminar desactiva isInvincible.
    private IEnumerator InvincibilityFlash()
    {
        yield return StartCoroutine(damageFlash.Flash());
        isInvincible = false;
    }

    /// Inicia la secuencia OnPreDeath → flash → Die().
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

    protected virtual void OnDamaged() { }

    protected virtual void OnKnockback(Vector2 sourcePosition) { }

    protected virtual void OnPreDeath() { }

    protected abstract void Die();
}