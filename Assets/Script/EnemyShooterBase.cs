using UnityEngine;

public abstract class EnemyShooterBase : Enemy
{
    [Header("Disparo")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float fireRate = 1f;

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
        if (!CanShoot()) return;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    protected bool CanShoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"[{GetType().Name}] bulletPrefab o firePoint no asignados en '{gameObject.name}'.");
            return false;
        }
        return true;
    }

    protected float DetectarDistancia(Transform objetivo)
    {
        if (objetivo == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, objetivo.position);
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