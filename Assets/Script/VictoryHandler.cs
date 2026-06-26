using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryHandler : MonoBehaviour
{
    [Header("Configuración")]
    public string nivelFinal = "Nivel1";
    public VictoryManager victoryManager; // Asigna si quieres mostrar el panel de victoria

    public void OnBossDeath()
    {
        if (SceneManager.GetActiveScene().name == nivelFinal)
        {
            if (victoryManager != null)
                victoryManager.MostrarVictoria();
            else
                Debug.LogWarning("VictoryManager no asignado en VictoryHandler.");
        }
        else
        {
            SceneManager.LoadScene(nivelFinal);
        }
    }
}       