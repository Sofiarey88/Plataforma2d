using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDeathManager : MonoBehaviour
{
    // Llama este método cuando el boss muera
    public void OnBossDeath()
    {
        SceneManager.LoadScene("Nivel1");
    }
}