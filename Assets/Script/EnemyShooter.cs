using UnityEngine;

/// <summary>
/// Enemigo estático que dispara proyectiles periódicamente.
/// El ángulo de salida se controla girando el GameObject 'firePoint' en el Editor
/// (eje Z en la Scene view o campo Rotation Z en el Inspector).
/// El proyectil instanciado hereda esa rotación y viaja en su transform.right.
/// </summary>
public class EnemyShooter : Enemy
{
    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private float nextFireTime;

    public override void Move() { }

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
            return; // ← bug fix: faltaba este return
        }

        // La rotación del firePoint en el editor define el ángulo de salida.
        // No se necesita ningún parámetro extra: rotar firePoint.Z = rotar la trayectoria.
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    // ── Gizmo de editor ─────────────────────────────────────
    // Se dibuja SOLO en la Scene view al seleccionar el objeto.
    // No tiene ningún efecto en runtime.

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        const float lineLength = 1.5f;
        const float arrowSize  = 0.2f;

        Vector3 origin    = firePoint.position;
        Vector3 direction = firePoint.right;           // igual a transform.right del proyectil
        Vector3 tip       = origin + direction * lineLength;

        // Línea principal de dirección
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, tip);

        // Cabeza de flecha (dos líneas diagonales)
        Vector3 arrowLeft  = tip - Quaternion.Euler(0, 0,  25f) * direction * arrowSize;
        Vector3 arrowRight = tip - Quaternion.Euler(0, 0, -25f) * direction * arrowSize;
        Gizmos.DrawLine(tip, arrowLeft);
        Gizmos.DrawLine(tip, arrowRight);

        // Punto de origen
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, 0.08f);
    }
}