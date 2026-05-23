using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los enemigos.
/// Hereda salud de Personaje e implementa IMovable para movimiento obligatorio en hijos.
/// </summary>
public abstract class Enemy : Personaje, IMovable
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public int damageToPlayer = 1;

    protected Rigidbody2D rb;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

    protected override void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Voltea el sprite según la dirección de movimiento.
    /// </summary>
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
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        player?.TakeDamage(damageToPlayer);
    }
}