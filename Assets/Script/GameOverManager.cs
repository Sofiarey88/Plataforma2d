using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private Player player;
    public GameObject gameOverPanel;

    private void OnEnable() => player.OnHealthChanged += HandleHealthChanged;
    private void OnDisable() => player.OnHealthChanged -= HandleHealthChanged;

    private void HandleHealthChanged(int actual, int maximo)
    {
        if (actual > 0) return;

        gameOverPanel.SetActive(true);
        // Se desuscribe para que no se llame de nuevo si TakeDamage
        // se invoca sobre un personaje ya muerto
        player.OnHealthChanged -= HandleHealthChanged;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}