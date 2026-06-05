using UnityEngine;

/// <summary>
/// Controla al jugador: movimiento, salto, plataformas móviles y recepción de daño.
/// </summary>
public class Player : Personaje, IMovable
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool movingPlatformCountsAsGround = true;

    [Header("Stomp")]
    public float stompBounceForce = 8f;

    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;
    private bool isGrounded;
    private bool justJumped;

    // Plataforma móvil
    private Transform platformTransform;
    private Vector2 relativePos;
    private bool onMovingPlatform;

    /// <summary>
    /// Velocidad capturada al inicio de FixedUpdate, ANTES de que el motor físico
    /// resuelva colisiones. Usada por StompTrigger para saber si el player caía
    /// en el momento exacto del impacto, independientemente de lo que resuelva la física.
    /// </summary>
    public Vector2 VelocityBeforePhysics { get; private set; }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        bool canJump = isGrounded || (movingPlatformCountsAsGround && onMovingPlatform);
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            onMovingPlatform = false;
            justJumped = true;
        }
    }

    void FixedUpdate()
    {
        // ⚠️ Capturar ANTES de Move() y ANTES de que la física resuelva este frame.
        // OnTriggerEnter2D dispara después del paso físico, cuando rb.linearVelocity
        // ya fue modificado por colisiones. Este snapshot preserva la velocidad real
        // con la que el player llegó al contacto.
        VelocityBeforePhysics = rb.linearVelocity;
        Move();
    }

    // --- IMovable ---
    public void Move()
    {
        if (justJumped)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
            justJumped = false;
            return;
        }

        if (onMovingPlatform && platformTransform != null && (isGrounded || movingPlatformCountsAsGround))
        {
            relativePos.x += moveInput * speed * Time.fixedDeltaTime;
            float targetX = platformTransform.position.x + relativePos.x;
            rb.MovePosition(new Vector2(targetX, rb.position.y));
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Aplica el impulso de rebote hacia arriba tras un pisotón exitoso.
    /// Llamado por StompTrigger.
    /// </summary>
    public void ApplyStompBounce()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
        animator?.SetTrigger("Stomp");
    }

    // --- Hooks de Personaje ---
    protected override void OnDamaged()
    {
        animator?.SetTrigger("Hurt");
    }

    protected override void Die()
    {
        Debug.Log("El jugador ha muerto.");
        Destroy(gameObject);
    }

    // ── Colisiones ──────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (collision.gameObject.CompareTag("Ground"))
                    isGrounded = true;

                if (collision.gameObject.CompareTag("MovingPlatform") ||
                    collision.gameObject.GetComponent<PlataformaMovilHorizontal>() != null)
                {
                    platformTransform = collision.transform;
                    relativePos = (Vector2)transform.position - (Vector2)platformTransform.position;
                    onMovingPlatform = true;
                }
                break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (collision.gameObject.CompareTag("Ground"))
                    isGrounded = true;

                if (!onMovingPlatform && (collision.gameObject.CompareTag("MovingPlatform") ||
                    collision.gameObject.GetComponent<PlataformaMovilHorizontal>() != null))
                {
                    platformTransform = collision.transform;
                    relativePos = (Vector2)transform.position - (Vector2)platformTransform.position;
                    onMovingPlatform = true;
                }
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;

        if (collision.gameObject.CompareTag("MovingPlatform") ||
            collision.gameObject.GetComponent<PlataformaMovilHorizontal>() != null)
        {
            onMovingPlatform = false;
            if (platformTransform == collision.transform)
                platformTransform = null;
        }
    }
}