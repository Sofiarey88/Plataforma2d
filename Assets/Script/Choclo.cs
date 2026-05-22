using UnityEngine;

/// <summary>
/// Choclo implementa ICollectible: al entrar el Player se recoge y sumar puntos.
/// Asegúrate de que el Choclo tiene un Collider2D con Is Trigger = true.
/// </summary>
public class Choclo : MonoBehaviour, ICollectible
{
    [Tooltip("Puntos que otorga este cholito")]
    public int value = 1;

    public int Value => value;

    public void Collect(GameObject collector)
    {
        // Sumar al ScoreManager (si existe) y destruir el objeto
        ScoreManager.Instance?.AddScore(value);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
}
