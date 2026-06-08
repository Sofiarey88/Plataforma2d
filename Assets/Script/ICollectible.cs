using UnityEngine;

// Interfaz para objetos que el jugador puede recolectar.
public interface ICollectible
{
    int Value { get; }
    void Collect(GameObject collector);
}