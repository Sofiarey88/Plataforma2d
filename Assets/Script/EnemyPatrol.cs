using UnityEngine;

/// <summary>
/// Enemigo que patrulla entre dos puntos (izquierda y derecha).
/// </summary>
public class EnemyPatrol : EnemyBase
{
    [Header("Patrulla")]
    public float patrolDistance = 3f;

    // Posición de inicio se actualiza en FixedUpdate para tolerar plataformas móviles
    private Vector2 originPosition;
    private bool originSet = false;
    private int direction = 1;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Move()
    {
        // Guardamos el origen la primera vez que el enemigo está apoyado en algo
        if (!originSet)
        {
            originPosition = rb.position;
            originSet = true;
        }

        rb.linearVelocity = new Vector2(moveSpeed * direction, rb.linearVelocity.y);

        float distanceTraveled = rb.position.x - originPosition.x;

        if (distanceTraveled >= patrolDistance && direction == 1)
        {
            direction = -1;
            Flip();
        }
        else if (distanceTraveled <= -patrolDistance && direction == -1)
        {
            direction = 1;
            Flip();
        }
    }
}