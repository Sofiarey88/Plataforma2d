using UnityEngine;

/// <summary>
/// Gestor de puntuación sencillo (Singleton).
/// Añade este componente a un GameObject vacío en la escena (por ejemplo "GameManager").
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log("Score: " + Score);
        // Aquí puedes emitir eventos o actualizar UI
    }
}