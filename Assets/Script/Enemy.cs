using UnityEngine;

public abstract class Enemy : Personaje, IMovable, IStompable
{
    [Header("Movimiento")]
    public float moveSpeed      = 2f;
    public int   damageToPlayer = 1;

    [Header("Stomp")]
    [Tooltip("Daño que recibe este enemigo por cada pisotón.\n" +
             "999 = muerte instantánea (comportamiento por defecto).\n" +
             "Para enemigos de varios pisotones: igualar a 1 y subir maxHealth.")]
    [SerializeField] private int stompDamage = 999;

    [Tooltip("Trigger del Animator al recibir un pisotón no letal. Dejar vacío para no disparar nada.")]
    [SerializeField] private string stompHitTrigger = "Hurt";

    protected Rigidbody2D rb;
    protected Animator    animator;
    protected bool        isFacingRight = true;

    protected virtual void Awake()
    {
        rb       = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected override void Start()       => base.Start();
    protected virtual  void FixedUpdate() => Move();

    // --- IMovable ---
    public abstract void Move();

    // --- IStompable ---
    /// <summary>
    /// Aplica stompDamage por cada pisotón.
    /// - stompDamage >= currentHealth → muerte inmediata (TriggerDeathSequence)
    /// - stompDamage < currentHealth  → pierde vida, flash, sigue vivo
    /// El comportamiento por defecto (stompDamage = 999) conserva la muerte instantánea.
    /// </summary>
    public virtual void OnStomp()
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - stompDamage);

        if (!IsAlive)
        {
            // Pisotón letal → secuencia de muerte con flash
            TriggerDeathSequence();
        }
        else
        {
            // Pisotón no letal → feedback visual sin morir
            if (damageFlash != null)
                StartCoroutine(damageFlash.Flash());

            if (!string.IsNullOrEmpty(stompHitTrigger))
                animator?.SetTrigger(stompHitTrigger);
        }
    }

    // --- Hooks de Personaje ---

    protected override void OnDamaged()  => animator?.SetTrigger("Hurt");

    protected override void OnPreDeath()
    {
        enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType       = RigidbodyType2D.Kinematic;
        }

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;
    }

    protected override void Die()
    {
        animator?.SetTrigger("Die");
        Debug.Log($"{gameObject.name} ha muerto.");
        Destroy(gameObject);
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// Pasa transform.position como sourcePosition para que Player
    /// calcule correctamente la dirección del knockback.
    /// </summary>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!IsAlive) return;

        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        player?.TakeDamage(damageToPlayer, transform.position); // ← fuente de daño
    }
}