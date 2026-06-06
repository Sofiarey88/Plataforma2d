using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Player player;
    public GameObject gameOverPanel;

    private bool gameOverMostrado = false;

    void Update()
    {
        if (!gameOverMostrado && player.CurrentHealth <= 0)
        {
            gameOverMostrado = true;
            gameOverPanel.SetActive(true);
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}