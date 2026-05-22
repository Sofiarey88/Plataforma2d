using UnityEngine;

/// <summary>
/// Interfaz para objetos que el jugador puede recolectar.
/// </summary>
public interface ICollectible
{
    int Value { get; }
    void Collect(GameObject collector);
}