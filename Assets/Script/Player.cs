using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private Vector3 escalaOriginal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Guardamos la escala original
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // Movimiento horizontal
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Girar personaje sin cambiar tamaño
        if (move > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(escalaOriginal.x),
                escalaOriginal.y,
                escalaOriginal.z);
        }
        else if (move < 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(escalaOriginal.x),
                escalaOriginal.y,
                escalaOriginal.z);
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Animaciones
        anim.SetFloat("Speed", Mathf.Abs(move));
        anim.SetFloat("YVelocity", rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

