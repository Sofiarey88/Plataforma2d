using UnityEngine;

/// <summary>
/// Controla al jugador: movimiento, salto, plataformas móviles y recepción de daño.
/// Reemplaza a PlayerController + PlayerHealth.
/// </summary>
public class Player : Personaje, IMovable
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 10f;
    public bool movingPlatformCountsAsGround = true;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool justJumped;

    // Plataforma móvil
    private Transform platformTransform;
    private Vector2 relativePos;
    private bool onMovingPlatform;

    protected override void Start()
    {
        base.Start(); // inicializa currentHealth
        rb = GetComponent<Rigidbody2D>();
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