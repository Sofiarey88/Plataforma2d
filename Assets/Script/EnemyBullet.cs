using UnityEngine;

// Proyectil físico disparado por EnemyShooter.
// Pasa su posición como sourcePosition para activar el knockback del Player.

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyBullet : Projectile
{
    [Header("Movimiento")]
    [Tooltip("Velocidad inicial en la dirección del firePoint")]
    public float speed = 8f;

    [Header("Rebotes")]
    [Tooltip("Rebotes máximos antes de destruirse (al rebote N+1 se destruye)")]
    [SerializeField] private int maxBounces = 2;

    [Header("Vida útil")]
    [Tooltip("Segundos hasta autodestrucción si no impacta con nada")]
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D rb;
    private int bounceCount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    private void Update() => AlignToVelocity();

    private void AlignToVelocity()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.01f) return;
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            player?.TakeDamage(damage, transform.position); // ← fuente de daño
            Destroy(gameObject);
            return;
        }

        bounceCount++;
        if (bounceCount > maxBounces)
            Destroy(gameObject);
    }
}