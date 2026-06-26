using System.Collections;
using UnityEngine;

/// Controla al jugador: movimiento, salto, animaciones, knockback e invencibilidad.
/// Hereda vida e IDamageable de Personaje. Implementa IMovable.
public class Player : Personaje, IMovable
{
    [Header("Movimiento")]
    public float speed      = 5f;
    public float jumpForce  = 8f;

    [Header("Stomp")]
    public float stompBounceForce = 8f;

    [Header("Knockback")]
    [Tooltip("Fuerza horizontal del empujón al recibir daño")]
    [SerializeField] private float knockbackForce = 6f;

    [Tooltip("Fuerza vertical del empujón al recibir daño")]
    [SerializeField] private float knockbackUpForce = 4f;

    [Tooltip("Segundos sin control horizontal tras recibir daño")]
    [SerializeField] private float knockbackControlLock = 0.2f;

    [Header("Bullet Time")]
    [Tooltip("Multiplicador de gravedad EXTRA durante el bullet time (1 = solo compensa el timeScale, >1 = más pesado)")]
    [SerializeField] private float bulletTimeGravityMultiplier = 2f;

    [Header("Animación")]
    [Tooltip("Parámetro float del Animator que recibe la velocidad horizontal")]
    public string moveParam    = "MotMen";

    [Tooltip("Trigger del Animator que activa la animación de salto")]
    public string jumpTrigger  = "Jump";

    [Tooltip("Bool del Animator que indica que estamos en suelo")]
    public string groundedParam = "Suelo";

    [Tooltip("Multiplicador aplicado a la velocidad X antes de enviarla al Animator")]
    public float moveMultiplier = 1f;

    [Tooltip("Umbral mínimo de velocidad para considerar que el jugador se está moviendo")]
    public float moveThreshold  = 0.1f;

    private Rigidbody2D rb;
    private Animator    anim;
    private bool        isGrounded;
    private Vector3     escalaOriginal;

    private float moveInput;
    private bool  jumpPressed;
    private bool  isKnockedBack;
    private float originalGravityScale;

    public Vector2 VelocityBeforePhysics { get; private set; }

    private bool bulletTimeActive;
    private Coroutine bulletTimeCoroutine;

    // ── Ciclo de vida ────────────────────────────────────────

    protected override void Start()
    {
        base.Start();
        rb                  = GetComponent<Rigidbody2D>();
        anim                = GetComponent<Animator>();
        escalaOriginal      = transform.localScale;
        originalGravityScale = rb != null ? rb.gravityScale : 1f;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0f)
            transform.localScale = new Vector3( Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else if (moveInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpPressed = true;

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        VelocityBeforePhysics = rb.linearVelocity;
        Move();
    }

    // ── IMovable ─────────────────────────────────────────────

    public void Move()
    {
        float compensatedSpeed = bulletTimeActive ? speed / Time.timeScale : speed;

        float compensatedJumpForce = bulletTimeActive
            ? jumpForce * Mathf.Sqrt(rb.gravityScale / originalGravityScale)
            : jumpForce;

        if (!isKnockedBack)
            rb.linearVelocity = new Vector2(moveInput * compensatedSpeed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, compensatedJumpForce);
            jumpPressed = false;
            isGrounded  = false;
            anim?.SetTrigger(jumpTrigger);
            anim?.SetBool(groundedParam, false);
        }
    }

    // ── Stomp ────────────────────────────────────────────────

    public void ApplyStompBounce()
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
        anim?.SetTrigger(jumpTrigger);
    }

    // ── Hooks de Personaje ───────────────────────────────────

    protected override void OnDamaged()
    {
        anim?.SetTrigger("Hurt");
    }

    protected override void OnKnockback(Vector2 sourcePosition)
    {
        if (rb == null) return;
        if (sourcePosition == default) return;

        Vector2 toPlayer = (Vector2)transform.position - sourcePosition;
        float xDir = toPlayer.sqrMagnitude > 0.001f
            ? Mathf.Sign(toPlayer.x)
            : (transform.localScale.x >= 0 ? -1f : 1f);

        rb.linearVelocity = new Vector2(xDir * knockbackForce, knockbackUpForce);
        StartCoroutine(KnockbackLock());
    }

    private IEnumerator KnockbackLock()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(knockbackControlLock);
        isKnockedBack = false;
    }

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

        anim.SetFloat(moveParam,     horVel * moveMultiplier);
        anim.SetFloat("YVelocity",  rb != null ? rb.linearVelocity.y : 0f);
        anim.SetBool("IsMoving",    horVel > moveThreshold);
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

    // ── Bullet Time ──────────────────────────────────────────

    public void StartBulletTime(float slowFactor, float duration)
    {
        if (bulletTimeCoroutine != null)
            StopCoroutine(bulletTimeCoroutine);

        bulletTimeCoroutine = StartCoroutine(BulletTimeCoroutine(slowFactor, duration));
    }

    private IEnumerator BulletTimeCoroutine(float slowFactor, float duration)
    {
        slowFactor = Mathf.Clamp(slowFactor, 0.05f, 0.99f);

        Time.timeScale       = slowFactor;
        Time.fixedDeltaTime  = 0.02f * slowFactor;
        bulletTimeActive     = true;

        if (rb != null)
            rb.gravityScale = originalGravityScale * (1f / slowFactor) * bulletTimeGravityMultiplier;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale      = 1f;
        Time.fixedDeltaTime = 0.02f;
        bulletTimeActive    = false;

        if (rb != null)
            rb.gravityScale = originalGravityScale;

        bulletTimeCoroutine = null;
    }

    private void OnDisable()
    {
        if (bulletTimeActive)
        {
            Time.timeScale      = 1f;
            Time.fixedDeltaTime = 0.02f;
            bulletTimeActive    = false;
        }
    }
}

