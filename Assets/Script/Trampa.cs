using UnityEngine;

/// Hazard del escenario. Al contacto con el jugador, aplica daño letal.
/// Implementa IInteractable: la lógica de interacción está desacoplada del trigger.
public class Trampa : MonoBehaviour, IInteractable
{
    [Tooltip("Daño que aplica la trampa al contacto (por defecto letal: iguala maxHealth)")]
    public bool instantKill = true;
    public int damage = 1;

    // --- IInteractable ---
    public void Interact(GameObject interactor)
    {
        IDamageable target = interactor.GetComponent<IDamageable>();
        if (target == null) return;

        int dmg = instantKill ? target.MaxHealth : damage;
        target.TakeDamage(dmg);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Interact(other.gameObject);
    }
}