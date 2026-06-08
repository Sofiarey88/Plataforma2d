using UnityEngine;

// Plataforma de lanzamiento vertical.
// Cuando alcanza su punto más alto y comienza a bajar, aplica un impulso
// ascendente al jugador. Al aterrizar de nuevo, cancela la velocidad de caída
// para evitar rebotes.
public class PlataformaLanzamiento : EscenarioMovil
{
    [Header("Lanzamiento")]
    [Tooltip("Multiplicador del impulso de lanzamiento. Mayor valor = más altura.")]
    public float launchMultiplier = 2f;

    [Tooltip("Velocidad mínima del descenso para activar el lanzamiento (evita micro-impulsos).")]
    public float launchThreshold = 0.01f;

    private float previousOffset = 0f;
    private bool hasLaunched = false;
    private Rigidbody2D playerRb;

    void Update()
    {
        if (!hasRb)
            Move();
    }

    void FixedUpdate()
    {
        if (hasRb)
            MoveByRigidbody(Time.fixedTime);
    }

    public override void Move()
    {
        if (distance <= 0f) return;

        float offset = Mathf.PingPong(Time.time * speed + distance, distance * 2f) - distance;
        float delta = offset - previousOffset;
        previousOffset = offset;

        Vector3 pos = startPos + Vector3.up * offset;
        if (useLocalPosition) transform.localPosition = pos;
        else transform.position = pos;

        TryLaunchPlayer(delta);
    }

    private void MoveByRigidbody(float time)
    {
        if (distance <= 0f || rb2d == null) return;

        float offset = Mathf.PingPong(time * speed + distance, distance * 2f) - distance;
        float delta = offset - previousOffset;
        previousOffset = offset;

        rb2d.MovePosition(new Vector2(rb2d.position.x, startPos.y + offset));

        TryLaunchPlayer(delta);
    }

    private void TryLaunchPlayer(float delta)
    {
        if (playerRb == null) return;

        if (delta < -launchThreshold && !hasLaunched)
        {
            float launchForce = speed * launchMultiplier;
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, launchForce);
            hasLaunched = true;
        }

        if (delta > launchThreshold)
            hasLaunched = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (playerRb != null)
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);

                hasLaunched = false;
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerRb = null;
    }
}