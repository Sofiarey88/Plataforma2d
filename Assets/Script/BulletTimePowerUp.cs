using UnityEngine;


// Power-up de Bullet Time: ralentiza todo el mundo excepto al jugador.
// El jugador compensa el timeScale internamente en Player para moverse con normalidad.

public class BulletTimePowerUp : PowerUp
{
    [Header("Bullet Time")]
    [Tooltip("Factor de lentitud del mundo (0.1 = 10%, 0.5 = mitad de velocidad)")]
    [Range(0.05f, 0.9f)]
    public float timeSlowFactor = 0.25f;

    [Tooltip("Duración del efecto en segundos reales (independiente del timeScale)")]
    public float duration = 5f;

    protected override void ApplyEffect(GameObject collector)
    {
        Player player = collector.GetComponent<Player>();
        player?.StartBulletTime(timeSlowFactor, duration);
    }
}