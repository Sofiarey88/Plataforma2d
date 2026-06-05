using UnityEngine;

/// <summary>
/// Clase base abstracta para proyectiles.
/// Implementa IStompable: puede ser destruido si el jugador lo pisa desde arriba.
/// La lógica de movimiento y colisiones se define en las clases concretas.
/// </summary>
public abstract class Projectile : MonoBehaviour, IStompable
{
    [Header("Proyectil")]
    [SerializeField] protected int damage = 1;

    // --- IStompable ---
    /// <summary>
    /// Destruye el proyectil al ser pisoteado.
    /// Las subclases pueden sobrescribir para añadir efectos o partículas.
    /// </summary>
    public virtual void OnStomp()
    {
        Destroy(gameObject);
    }
}