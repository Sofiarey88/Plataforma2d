using UnityEngine;

// Clase base para enemigos que disparan: centraliza Update/Shoot/Gizmos.
public abstract class ShooterEnemy : Enemy
{
    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    [Tooltip("Si se activa, la bala instanciada heredara el valor de damageToPlayer de este enemigo.")]
    public bool inheritDamageToBullet = true;

    private float nextFireTime;

    protected virtual void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    protected virtual void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"[{GetType().Name}] bulletPrefab o firePoint no asignados en '{gameObject.name}'.");
            return;
        }

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (inheritDamageToBullet && bulletObj.TryGetComponent<Projectile>(out var bullet))
        {
            bullet.SetDamage(damageToPlayer);
        }
    }

    protected virtual void OnDrawGizmosSelected()
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