using UnityEngine;

/// <summary>
/// Controla al jugador: movimiento, salto, animaciones y recepción de daño.
/// Hereda vida e IDamageable de Personaje. Implementa IMovable para el sistema de movimiento.
/// </summary>
public class Player : Personaje, IMovable
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("Stomp")]
    public float stompBounceForce = 8f;

    [Header("Animación")]
    [Tooltip("Parámetro float del Animator que recibe la velocidad horizontal")]
    public string moveParam = "MotMen";

    [Tooltip("Trigger del Animator que activa la animación de salto")]
    public string jumpTrigger = "Jump";

    [Tooltip("Bool del Animator que indica que estamos en suelo")]
    public string groundedParam = "Suelo";

    [Tooltip("Multiplicador aplicado a la velocidad X antes de enviarla al Animator")]
    public float moveMultiplier = 1f;

    [Tooltip("Umbral mínimo de velocidad para considerar que el jugador se está moviendo")]
    public float moveThreshold = 0.1f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private Vector3 escalaOriginal;

    // Entrada capturada en Update, aplicada en FixedUpdate
    private float moveInput;
    private bool jumpPressed;

    /// <summary>
    /// Velocidad capturada al INICIO de FixedUpdate, antes de que el motor físico
    /// resuelva colisiones. StompTrigger la usa para confirmar que el player
    /// estaba cayendo en el momento exacto del impacto.
    /// </summary>
    public Vector2 VelocityBeforePhysics { get; private set; }

    // ── Ciclo de vida ────────────────────────────────────────

    protected override void Start()
    {
        base.Start(); // inicializa currentHealth en Personaje
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip de sprite según dirección
        if (moveInput > 0f)
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else if (moveInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpPressed = true;

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // ⚠️ Snapshot ANTES de Move() y ANTES de que la física resuelva colisiones.
        // OnTriggerEnter2D dispara después del paso físico: rb.linearVelocity.y ya
        // es 0 en ese momento. Este valor preserva la velocidad real de caída.
        VelocityBeforePhysics = rb.linearVelocity;

        Move();
    }

    // ── IMovable ─────────────────────────────────────────────

    /// <summary>
    /// Aplica el movimiento horizontal y el salto al Rigidbody.
    /// Centraliza toda la física en un único método, respetando IMovable.
    /// </summary>
    public void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            isGrounded = false;

            anim?.SetTrigger(jumpTrigger);
            anim?.SetBool(groundedParam, false);
        }
    }

    // ── Stomp ────────────────────────────────────────────────

    /// <summary>
    /// Impulsa al player hacia arriba tras pisar un IStompable.
    /// Llamado por StompTrigger. Reutiliza la animación de salto.
    /// </summary>
    public void ApplyStompBounce()
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
        anim?.SetTrigger(jumpTrigger);
    }

    // ── Hooks de Personaje ───────────────────────────────────

    /// <summary>
    /// Dispara la animación de daño al recibir un golpe lateral.
    /// Trigger "Hurt" → configurar en el Animator del Player.
    /// </summary>
    protected override void OnDamaged()
    {
        anim?.SetTrigger("Hurt");
    }

    /// <summary>
    /// Destruye al jugador cuando su vida llega a 0.
    /// </summary>
    protected override void Die()
    {
        Debug.Log("El jugador ha muerto.");
        Destroy(gameObject);
    }

    // ── Animaciones ──────────────────────────────────────────

    private void UpdateAnimator()
    {
        if (anim == null) return;

        float horVel = rb != null
            ? Mathf.Abs(rb.linearVelocity.x)
            : Mathf.Abs(moveInput * speed);

        anim.SetFloat(moveParam, horVel * moveMultiplier);
        anim.SetFloat("YVelocity", rb != null ? rb.linearVelocity.y : 0f);
        anim.SetBool("IsMoving", horVel > moveThreshold);
        anim.SetBool(groundedParam, isGrounded);
    }

    // ── Colisiones ───────────────────────────────────────────

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f &&
                (collision.gameObject.CompareTag("Ground") ||
                 collision.gameObject.CompareTag("MovingPlatform")))
            {
                isGrounded = true;
                anim?.SetBool(groundedParam, true);
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") ||
            collision.gameObject.CompareTag("MovingPlatform"))
        {
            isGrounded = false;
            anim?.SetBool(groundedParam, false);
        }
    }
}

