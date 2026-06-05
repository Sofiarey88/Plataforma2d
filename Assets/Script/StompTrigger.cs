using UnityEngine;

/// <summary>
/// Trigger en la parte inferior del Player que detecta pisotones sobre IStompable.
/// Adjuntar a un GameObject hijo del Player con un Collider2D en la base del sprite.
/// </summary>
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

        // Usar la velocidad pre-física: en el momento que el player tocó al enemigo,
        // la física ya resolvió rb.linearVelocity a 0. VelocityBeforePhysics conserva
        // la velocidad de caída real con la que llegó este frame.
        if (player.VelocityBeforePhysics.y >= 0f) return;

        IStompable stompable = other.GetComponent<IStompable>();
        if (stompable == null) return;

        stompable.OnStomp();
        player.ApplyStompBounce();
    }
}