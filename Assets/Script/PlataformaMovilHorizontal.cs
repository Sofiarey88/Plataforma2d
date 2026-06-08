using UnityEngine;

/// Plataforma móvil con desplazamiento horizontal.
/// Siempre inicia el movimiento desde la posición exacta donde fue colocada en escena.
public class PlataformaMovilHorizontal : EscenarioMovil
{
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

    // --- IMovable ---
    public override void Move()
    {
        if (distance <= 0f) return;
        float offset = Mathf.PingPong(Time.time * speed + distance, distance * 2f) - distance;
        Vector3 pos = startPos + Vector3.right * offset;

        if (useLocalPosition) transform.localPosition = pos;
        else transform.position = pos;
    }

    private void MoveByRigidbody(float time)
    {
        if (distance <= 0f || rb2d == null) return;
        float offset = Mathf.PingPong(time * speed + distance, distance * 2f) - distance;
        rb2d.MovePosition(new Vector2(startPos.x + offset, rb2d.position.y));
    }
}