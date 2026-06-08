using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI textoPuntos;
    private int score = 0;

    private void Awake()
    {
        Instance = this;
        textoPuntos.text = "Choclos: 0";
    }

    private void OnEnable() => ScoreEvents.OnPuntosRecolectados += AddScore;
    private void OnDisable() => ScoreEvents.OnPuntosRecolectados -= AddScore;

    public void AddScore(int puntos)
    {
        score += puntos;
        textoPuntos.text = "Choclos: " + score;
    }
}