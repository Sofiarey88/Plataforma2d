using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed = 5f;
    public float jumpForce = 10f;

    // Si es true, las plataformas con tag "MovingPlatform" contarán como suelo para saltar.
    public bool movingPlatformCountsAsGround = true;

    private float move;
    private bool isGrounded;

    // Plataforma sobre la que estamos (si hay)
    private Transform platformTransform;
    private Vector2 relativePos;
    private bool onMovingPlatform;

    // Evita que FixedUpdate anule el salto inmediatamente
    private bool justJumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");

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
        if (justJumped)
        {
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
            justJumped = false;
            return;
        }

        if (onMovingPlatform && platformTransform != null && (isGrounded || movingPlatformCountsAsGround))
        {
            relativePos.x += move * speed * Time.fixedDeltaTime;
            float targetX = platformTransform.position.x + relativePos.x;
            Vector2 targetPos = new Vector2(targetX, rb.position.y);
            rb.MovePosition(targetPos);
            return;
        }

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                // Solo marcamos isGrounded si la tag es Ground
                if (collision.gameObject.CompareTag("Ground"))
                    isGrounded = true;

                // Detectamos plataforma móvil por tag o por componente (no la convertimos en Ground a menos que la opción esté activada)
                if (collision.gameObject.CompareTag("MovingPlatform") || collision.gameObject.GetComponent<PlataformMov>() != null)
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

                if (!onMovingPlatform && (collision.gameObject.CompareTag("MovingPlatform") || collision.gameObject.GetComponent<PlataformMov>() != null))
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
        if (platformTransform == collision.transform &&
            (collision.gameObject.CompareTag("MovingPlatform") || collision.gameObject.GetComponent<PlataformMov>() != null))
        {
            platformTransform = null;
            onMovingPlatform = false;
        }

        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;

        if (collision.gameObject.CompareTag("MovingPlatform") || collision.gameObject.GetComponent<PlataformMov>() != null)
            onMovingPlatform = false;
    }
}