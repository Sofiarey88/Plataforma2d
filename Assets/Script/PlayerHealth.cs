using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    private int currentLives;

    void Start()
    {
        currentLives = maxLives;
    }

    public void TakeDamage(int amount = 1)
    {
        currentLives -= amount;
        Debug.Log($"Vidas restantes: {currentLives}");

        if (currentLives <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        Destroy(gameObject);
    }

    public int GetCurrentLives() => currentLives;
}