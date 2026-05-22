using UnityEngine;

/// <summary>
/// Plataforma móvil simple: se desplaza horizontalmente usando los campos de la clase base EscenarioMovil.
/// Funciona con transform (Update) o con Rigidbody2D.MovePosition (FixedUpdate) si existe Rigidbody2D.
/// </summary>
public class PlataformMov : EscenarioMovil
{
    void Update()
    {
        if (!hasRb)
            MoveByTransform(Time.time);
    }

    void FixedUpdate()
    {
        if (hasRb)
            MoveByRigidbody(Time.fixedTime);
    }

    void MoveByTransform(float time)
    {
        if (distance <= 0f) return;
        float offset = Mathf.PingPong(time * speed, distance * 2f) - distance;
        Vector3 pos = startPos + Vector3.right * offset;
        if (useLocalPosition) transform.localPosition = pos;
        else transform.position = pos;
    }

    void MoveByRigidbody(float time)
    {
        if (distance <= 0f || rb2d == null) return;
        float offset = Mathf.PingPong(time * speed, distance * 2f) - distance;
        Vector2 target = new Vector2(startPos.x + offset, rb2d.position.y);
        rb2d.MovePosition(target);
    }
}
