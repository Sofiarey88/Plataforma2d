using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDeathManager : MonoBehaviour
{
    // Llama este m�todo cuando el boss muera
    public void OnBossDeath()
    {
        SceneManager.LoadScene("Nivel1");
    }
}