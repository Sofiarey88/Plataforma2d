using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    public GameObject victoryPanel;

    public void MostrarVictoria()
    {
        victoryPanel.SetActive(true);
    }
}