using UnityEngine;

/// <summary>
/// Base para objetos de escenario con movimiento.
/// Inicializa posición, Rigidbody2D y flags comunes.
/// </summary>
public abstract class EscenarioMovil : MonoBehaviour
{
    [Header("Movimiento común")]
    [Tooltip("Velocidad del movimiento (unidades/seg)")]
    public float speed = 2f;

    [Tooltip("Distancia máxima desde la posición inicial hacia cada lado")]
    public float distance = 3f;

    [Tooltip("Usar posición local (no compatible con Rigidbody2D)")]
    public bool useLocalPosition = false;

    [Tooltip("Si hay Rigidbody2D y está activado, usar MovePosition (recomendado para físicas)")]
    public bool useRigidbody2D = true;

    protected Vector3 startPos;
    protected Rigidbody2D rb2d;
    protected bool hasRb;

    // Usamos Awake para inicializar la base (Unity llama a Awake antes que Start)
    protected virtual void Awake()
    {
        startPos = useLocalPosition ? transform.localPosition : transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        hasRb = rb2d != null && useRigidbody2D;

        if (hasRb && useLocalPosition)
        {
            Debug.LogWarning("EscenarioMovil: useLocalPosition no es compatible con Rigidbody2D. Se usará transform en su lugar.");
            hasRb = false;
        }

        if (hasRb)
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.freezeRotation = true;
        }
    }
}