using UnityEngine;

// Clase base abstracta para proyectiles.
// Implementa IStompable: puede ser destruido si el jugador lo pisa desde arriba.
// La lógica de movimiento y colisiones se define en las clases concretas.
public abstract class Projectile : MonoBehaviour, IStompable
{
    [Header("Proyectil")]
    [SerializeField] protected int damage = 1;

    // --- IStompable ---
    // Destruye el proyectil al ser pisoteado.
    public virtual void OnStomp()
    {
        Destroy(gameObject);
    }
}