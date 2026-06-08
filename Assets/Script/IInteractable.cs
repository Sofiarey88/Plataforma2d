using UnityEngine;

// Objetos del escenario con los que el jugador u otras entidades pueden interactuar.
public interface IInteractable
{
    void Interact(GameObject interactor);
}