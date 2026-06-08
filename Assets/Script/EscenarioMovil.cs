using UnityEngine;

// Clase base abstracta para objetos del escenario con movimiento.
// Implementa IMovable: las subclases definen Move().
public abstract class EscenarioMovil : MonoBehaviour, IMovable
{
    [Header("Movimiento común")]
    [Tooltip("Velocidad (unidades/seg)")]
    public float speed = 2f;

    [Tooltip("Distancia máxima desde la posición inicial hacia cada lado")]
    public float distance = 3f;

    [Tooltip("Usar posición local (no compatible con Rigidbody2D)")]
    public bool useLocalPosition = false;

    [Tooltip("Usar Rigidbody2D.MovePosition si existe un Rigidbody2D (recomendado para físicas)")]
    public bool useRigidbody2D = true;

    protected Vector3 startPos;
    protected Rigidbody2D rb2d;
    protected bool hasRb;

    protected virtual void Awake()
    {
        startPos = useLocalPosition ? transform.localPosition : transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        hasRb = rb2d != null && useRigidbody2D;

        if (hasRb && useLocalPosition)
        {
            Debug.LogWarning("EscenarioMovil: useLocalPosition no es compatible con Rigidbody2D. Se usará transform.");
            hasRb = false;
        }

        if (hasRb)
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            rb2d.freezeRotation = true;
        }
    }

    public abstract void Move();
}