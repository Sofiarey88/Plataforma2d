using UnityEngine;

// Único heredero de Enemy que SE mueve, por eso es el único
// que firma el contrato IMovable.
public class EnemyPatrol : Enemy, IMovable
{
    [Header("Patrulla")]
    public float patrolDistance = 3f;

    private Vector2 originPosition;
    private bool originSet = false;
    private int direction = 1;

    public void Move()
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

    private void FixedUpdate() => Move();
}