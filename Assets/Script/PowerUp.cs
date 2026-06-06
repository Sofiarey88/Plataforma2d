using UnityEngine;

/// <summary>
/// Clase base abstracta para todos los power-ups.
/// Implementa ICollectible (recogible por trigger) e IMovable (movimiento en figura de ocho).
/// Las subclases definen el efecto concreto en ApplyEffect().
/// Requiere un Collider2D con Is Trigger = true en el GameObject.
/// </summary>
public abstract class PowerUp : MonoBehaviour, ICollectible, IMovable
{
    [Header("Collectible")]
    [Tooltip("Puntos que otorga al recogerse (0 si no aporta puntuación)")]
    [SerializeField] private int value = 0;
    public int Value => value;

    [Header("Movimiento en ocho (Lissajous)")]
    [Tooltip("Velocidad del ciclo completo del movimiento")]
    public float moveSpeed = 1.5f;

    [Tooltip("Amplitud horizontal del ocho")]
    public float amplitudeX = 0.8f;

    [Tooltip("Amplitud vertical del ocho")]
    public float amplitudeY = 0.4f;

    private Vector3 startPos;

    protected virtual void Awake()
    {
        startPos = transform.position;
    }

    protected virtual void Update()
    {
        Move();
    }

    // --- IMovable ---
    /// <summary>
    /// Curva de Lissajous: x = A·sin(t) / y = B·sin(2t) → dibuja un ocho.
    /// Usa tiempo no escalado para que el movimiento visual sea fluido
    /// incluso si el timeScale del mundo está modificado (ej: bullet time activo).
    /// </summary>
    public void Move()
    {
        float t = Time.unscaledTime * moveSpeed;
        float x = amplitudeX * Mathf.Sin(t);
        float y = amplitudeY * Mathf.Sin(2f * t);
        transform.position = startPos + new Vector3(x, y, 0f);
    }

    // --- ICollectible ---
    public void Collect(GameObject collector)
    {
        if (value > 0)
            ScoreManager.Instance?.AddScore(value);

        ApplyEffect(collector);
        Destroy(gameObject);
    }

    /// <summary>
    /// Efecto concreto del power-up. Obligatorio implementar en subclases.
    /// </summary>
    protected abstract void ApplyEffect(GameObject collector);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Collect(other.gameObject);
    }
}