using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los enemigos.
/// Hereda salud de Personaje e implementa IMovable y IStompable.
/// </summary>
public abstract class Enemy : Personaje, IMovable, IStompable
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public int damageToPlayer = 1;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    // --- IMovable ---
    public abstract void Move();

    // --- IStompable ---
    /// <summary>
    /// Mata al enemigo instantáneamente. Evita el trigger "Hurt" y va directo a Die().
    /// </summary>
    public virtual void OnStomp()
    {
        if (!IsAlive) return;
        currentHealth = 0;
        Die();
    }

    // --- Hooks de Personaje ---
    protected override void OnDamaged()
    {
        animator?.SetTrigger("Hurt");
    }

    /// <summary>
    /// Mata al enemigo: detiene su física, desactiva sus colliders de forma inmediata
    /// para que el player no quede físicamente parado sobre el cadáver, y destruye el objeto.
    /// </summary>
    protected override void Die()
    {
        // 1. Detener toda lógica propia (Update, FixedUpdate, callbacks de colisión)
        enabled = false;

        // 2. Congelar la física para que no se mueva ni caiga durante la muerte
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 3. ⚠️ Desactivar colliders INMEDIATAMENTE:
        //    - El player deja de estar "parado" sobre el enemigo → puede saltar
        //    - El StompTrigger no vuelve a activarse sobre este objeto
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        // 4. Animación de muerte (si existe en el Animator)
        animator?.SetTrigger("Die");

        Debug.Log($"{gameObject.name} ha muerto.");

        // 5. Destrucción inmediata (sin delay hasta que haya animación de muerte configurada).
        //    Para añadir delay cuando tengas la animación: Destroy(gameObject, duracionAnimacion);
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
    /// Daña al player si la colisión es lateral o desde abajo.
    /// El guard !IsAlive evita que un enemigo ya muerto aplique daño.
    /// </summary>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!IsAlive) return;

        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        player?.TakeDamage(damageToPlayer);
    }
}