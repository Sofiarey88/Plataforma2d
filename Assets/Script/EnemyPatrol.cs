using UnityEngine;

/// <summary>
/// Enemigo que patrulla entre dos puntos horizontales.
/// </summary>
public class EnemyPatrol : Enemy
{
    [Header("Patrulla")]
    public float patrolDistance = 3f;

    private Vector2 originPosition;
    private bool originSet = false;
    private int direction = 1;

    public override void Move()
    {
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