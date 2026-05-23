using UnityEngine;

/// <summary>
/// Plataforma móvil vertical. Cuando desciende, transfiere la velocidad de bajada
/// al jugador solo en el eje Y, sin restringir su movimiento horizontal.
/// </summary>
public class PlataformaMovilVertical : EscenarioMovil
{
    private float previousOffset = 0f;
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

        PushPlayerDown(delta);
    }

    private void MoveByRigidbody(float time)
    {
        if (distance <= 0f || rb2d == null) return;

        float offset = Mathf.PingPong(time * speed + distance, distance * 2f) - distance;
        float delta = offset - previousOffset;
        previousOffset = offset;

        rb2d.MovePosition(new Vector2(rb2d.position.x, startPos.y + offset));

        PushPlayerDown(delta);
    }

    /// <summary>
    /// Cuando la plataforma baja, impone la velocidad de descenso en Y al jugador
    /// sin tocar su velocidad horizontal.
    /// </summary>
    private void PushPlayerDown(float delta)
    {
        if (delta >= 0f || playerRb == null) return;

        // Velocidad de descenso de la plataforma en este frame
        float platformVelocityY = delta / Time.fixedDeltaTime;

        // Solo sobreescribimos Y si el jugador va más rápido que la plataforma (hacia arriba)
        if (playerRb.linearVelocity.y > platformVelocityY)
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, platformVelocityY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerRb = null;
    }
}