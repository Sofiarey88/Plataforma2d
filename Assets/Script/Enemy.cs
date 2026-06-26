using UnityEngine;

public abstract class Enemy : Personaje, IStompable
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public int damageToPlayer = 1;

    [Header("Stomp")]
    [SerializeField] private int stompDamage = 999;
    [SerializeField] private string stompHitTrigger = "Hurt";

    protected Rigidbody2D rb;
    protected Animator animator;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected override void Start() => base.Start();

    public virtual void OnStomp()
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - stompDamage);

        if (!IsAlive)
        {
            TriggerDeathSequence();
        }
        else
        {
            if (damageFlash != null)
                StartCoroutine(damageFlash.Flash());

            if (!string.IsNullOrEmpty(stompHitTrigger))
                animator?.SetTrigger(stompHitTrigger);
        }
    }

    // --- Hooks de Personaje ---
    protected override void OnDamaged() => animator?.SetTrigger("Hurt");

    protected override void OnPreDeath()
    {
        enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;
    }

    protected override void Die()
    {
        animator?.SetTrigger("Die");
        Destroy(gameObject);
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!IsAlive) return;

        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        player?.TakeDamage(damageToPlayer, transform.position);
    }
}