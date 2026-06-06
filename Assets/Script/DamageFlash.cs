using System.Collections;
using UnityEngine;

/// <summary>
/// Efecto visual de daño: parpadeo entre blanco y transparente.
/// - Fase ON  (blanco):      visible en sprites de colores, puede ser invisible en sprites blancos.
/// - Fase OFF (alpha = 0):  SIEMPRE visible, garantiza feedback en cualquier sprite.
/// Añadir al mismo GameObject (o hijo) donde estén los SpriteRenderers.
/// </summary>
public class DamageFlash : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Color del flash. Blanco por defecto; cámbialo si tus sprites ya son blancos")]
    [SerializeField] private Color flashColor = Color.white;

    [Tooltip("Número de parpadeos")]
    [SerializeField] private int flashCount = 2;

    [Tooltip("Duración en segundos de cada semiciclo (ON blanco / OFF transparente)")]
    [SerializeField] private float flashDuration = 0.08f;

    private SpriteRenderer[] renderers;
    private Color[]          originalColors;

    private void Awake()
    {
        // includeInactive:true → encuentra renderers aunque el hijo esté inactivo al inicio
        renderers      = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;

        if (renderers.Length == 0)
            Debug.LogWarning($"[DamageFlash] No hay SpriteRenderers en '{gameObject.name}' ni en sus hijos.");
    }

    /// <summary>
    /// Coroutine del parpadeo. Personaje hace yield aquí antes de llamar a Die().
    /// Secuencia por cada ciclo: blanco → transparente.
    /// Termina siempre restaurando el color original.
    /// </summary>
    public IEnumerator Flash()
    {
        for (int i = 0; i < flashCount; i++)
        {
            // Fase ON: blanco (visible si el sprite tiene colores distintos al blanco)
            SetColor(flashColor);
            yield return new WaitForSeconds(flashDuration);

            // Fase OFF: transparente — SIEMPRE visible independientemente del color del sprite
            SetTransparent();
            yield return new WaitForSeconds(flashDuration);
        }

        Restore();
    }

    // ── Helpers ─────────────────────────────────────────────

    private void SetColor(Color color)
    {
        foreach (SpriteRenderer sr in renderers)
            if (sr != null) sr.color = color;
    }

    private void SetTransparent()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].color = new Color(
                    originalColors[i].r,
                    originalColors[i].g,
                    originalColors[i].b,
                    0f);
        }
    }

    private void Restore()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null)
                renderers[i].color = originalColors[i];
    }

    // Seguridad: si el objeto se desactiva a mitad del flash, restaurar colores
    private void OnDisable() => Restore();
}