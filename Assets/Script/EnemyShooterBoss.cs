using UnityEngine;

// Enemigo estático boss. No implementa IMovable porque no se mueve.
// Requiere dos pisotones para morir 
public class EnemyShooterBoss : Enemy
{
    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("Victoria")]
    public GameObject victoryPanel;

    private float nextFireTime;

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"[EnemyShooterBoss] bulletPrefab o firePoint no asignados en '{gameObject.name}'.");
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (!IsAlive) return;

        foreach (ContactPoint2D contact in collision.contacts)
            if (contact.normal.y < -0.5f) return;

        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        player?.TakeDamage(damageToPlayer, transform.position);
    }

    protected override void Die()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        const float lineLength = 1.5f;
        const float arrowSize = 0.2f;

        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.right;
        Vector3 tip = origin + direction * lineLength;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, tip);
        Gizmos.DrawLine(tip, tip - Quaternion.Euler(0, 0, 25f) * direction * arrowSize);
        Gizmos.DrawLine(tip, tip - Quaternion.Euler(0, 0, -25f) * direction * arrowSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.08f);
    }
}