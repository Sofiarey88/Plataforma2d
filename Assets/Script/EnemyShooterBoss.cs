using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyShooterBoss : EnemyShooterBase
{
    [Header("Victoria")]
    public GameObject victoryPanel;

    [Header("Managers")]
    public BossDeathManager bossDeathManager;
    public VictoryManager victoryManager;

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
        if (bossDeathManager != null)
            bossDeathManager.OnBossDeath();

        if (SceneManager.GetActiveScene().name == "Nivel1" && victoryManager != null)
            victoryManager.MostrarVictoria();

        Destroy(gameObject);
    }
}