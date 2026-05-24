using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private Vector3 escalaOriginal;

    // entradas y flags para FixedUpdate
    private float moveInput;
    private bool jumpPressed;

    [Tooltip("Parámetro float del Animator que recibe la velocidad horizontal")]
    public string moveParam = "MotMen";

    [Tooltip("Multiplicador aplicado a la velocidad X antes de enviarla al Animator")]
    public float moveMultiplier = 1f;

    [Tooltip("Umbral para considerar que el jugador se está moviendo")]
    public float moveThreshold = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        // Leer entrada (sin aplicar física)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Girar personaje sin cambiar tamaño
        if (moveInput > 0f)
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else if (moveInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

        // Detectar salto (marcar para FixedUpdate)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpPressed = true;

        // Actualizar parámetros del Animator
        if (anim != null)
        {
            float horVel = rb != null ? Mathf.Abs(rb.linearVelocity.x) : Mathf.Abs(moveInput * speed);
            anim.SetFloat(moveParam, horVel * moveMultiplier); // envío MotMen = |velX| * multiplicador
            anim.SetFloat("YVelocity", rb != null ? rb.linearVelocity.y : 0f);
            anim.SetBool("IsMoving", horVel > moveThreshold);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Movimiento horizontal físico
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Aplicar salto cuando se solicitó
        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f &&
                (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform")))
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform"))
            isGrounded = false;
    }
}

