using UnityEngine;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private Player player;

    public GameObject vida1;
    public GameObject vida2;
    public GameObject vida3;

    private void OnEnable() => player.OnHealthChanged += Actualizar;
    private void OnDisable() => player.OnHealthChanged -= Actualizar;

    // Inicializa la UI con el estado actual sin esperar al primer daño
    private void Start() => Actualizar(player.CurrentHealth, player.MaxHealth);

    private void Actualizar(int actual, int maximo)
    {
        vida1.SetActive(actual >= 1);
        vida2.SetActive(actual >= 2);
        vida3.SetActive(actual >= 3);
    }

}