using UnityEngine;

// Trigger en la parte inferior del Player que detecta pisotones sobre IStompable.
[RequireComponent(typeof(Collider2D))]
public class StompTrigger : MonoBehaviour
{
    private Player player;
    private Collider2D stompCollider;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        stompCollider = GetComponent<Collider2D>();
        stompCollider.isTrigger = true;

        if (player == null)
            Debug.LogError($"[StompTrigger] No se encontró el componente Player " +
                           $"en el padre de '{gameObject.name}'. " +
                           $"Asegúrate de que este objeto es hijo del Player.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (player == null) return;

        if (player.VelocityBeforePhysics.y >= 0f) return;

        IStompable stompable = other.GetComponent<IStompable>();
        if (stompable == null) return;

        stompable.OnStomp();
        player.ApplyStompBounce();
    }
}