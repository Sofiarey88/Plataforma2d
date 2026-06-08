using UnityEngine;

public class Choclo : MonoBehaviour, ICollectible
{
    [Tooltip("Puntos que otorga este cholito")]
    public int value = 1;
    public int Value => value;

    public void Collect(GameObject collector)
    {
        ScoreEvents.Notificar(value);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Collect(other.gameObject);
    }
}