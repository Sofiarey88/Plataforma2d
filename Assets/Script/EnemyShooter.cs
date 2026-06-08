using UnityEngine;

// Enemigo estático disparador. No implementa IMovable porque no se mueve.
public class EnemyShooter : Enemy
{
    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

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
            Debug.LogWarning($"[EnemyShooter] bulletPrefab o firePoint no asignados en '{gameObject.name}'.");
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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