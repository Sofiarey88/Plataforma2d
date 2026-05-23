using UnityEngine;

/// <summary>
/// Objetos del escenario con los que el jugador u otras entidades pueden interactuar.
/// </summary>
public interface IInteractable
{
    void Interact(GameObject interactor);
}