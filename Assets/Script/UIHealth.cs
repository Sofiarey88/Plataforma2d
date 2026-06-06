using UnityEngine;

public class UIHealth : MonoBehaviour
{
    public Player player;

    public GameObject vida1;
    public GameObject vida2;
    public GameObject vida3;

    void Update()
    {
        int vida = player.CurrentHealth;

        vida1.SetActive(vida >= 1);
        vida2.SetActive(vida >= 2);
        vida3.SetActive(vida >= 3);
    }
}