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

    [Tooltip("Trigger del Animator que activa la animación de salto")]
    public string jumpTrigger = "Jump";

    [Tooltip("Bool del Animator que indica que estamos en suelo (usar en transiciones de regreso)")]
    public string groundedParam = "Suelo";

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
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0f)
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else if (moveInput < 0f)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            jumpPressed = true;

        if (anim != null)
        {
            float horVel = rb != null ? Mathf.Abs(rb.linearVelocity.x) : Mathf.Abs(moveInput * speed);
            anim.SetFloat(moveParam, horVel * moveMultiplier);
            anim.SetFloat("YVelocity", rb != null ? rb.linearVelocity.y : 0f);
            anim.SetBool("IsMoving", horVel > moveThreshold);
            // mantener parámetro Suelo acorde al estado físico
            anim.SetBool(groundedParam, isGrounded);
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            isGrounded = false;
            // activar trigger de salto y actualizar Suelo=false
            if (anim != null)
            {
                anim.SetTrigger(jumpTrigger);
                anim.SetBool(groundedParam, false);
            }
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
                if (anim != null) anim.SetBool(groundedParam, true);
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform"))
        {
            isGrounded = false;
            if (anim != null) anim.SetBool(groundedParam, false);
        }
    }
}

