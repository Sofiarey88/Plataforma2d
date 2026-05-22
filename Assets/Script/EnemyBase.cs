using UnityEngine;

/// <summary>
/// Clase base para todos los enemigos del juego.
/// Las clases hijas deben sobreescribir Move() y optionalmente OnPlayerContact().
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 1;
    public int damageToPlayer = 1;
    protected int currentHealth;

    [Header("Movimiento")]
    public float moveSpeed = 2f;

    protected Rigidbody2D rb;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // Move() ahora se llama en FixedUpdate para sincronizar con el motor de física
    protected virtual void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Lógica de movimiento. Obligatorio sobreescribir en clases hijas.
    /// </summary>
    protected abstract void Move();

    /// <summary>
    /// Recibe daño. Se puede sobreescribir para efectos especiales.
    /// </summary>
    public virtual void TakeDamage(int amount = 1)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// Destruye al enemigo. Se puede sobreescribir para añadir animación, drops, etc.
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Voltea el sprite del enemigo según la dirección.
    /// </summary>
    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// Al entrar en contacto con el jugador, le aplica daño.
    /// Se puede sobreescribir para comportamientos especiales.
    /// </summary>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        playerHealth?.TakeDamage(damageToPlayer);
    }
}