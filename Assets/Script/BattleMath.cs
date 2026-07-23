using UnityEngine;

// Utilidades puras para cálculos reutilizables (devuelven valores).
public static class BattleMath
{
    // Calcula el daño final aplicando un multiplicador y redondeando.
    public static int CalculateDamage(int baseDamage, float multiplier)
    {
        return Mathf.Max(0, Mathf.RoundToInt(baseDamage * multiplier));
    }

    // Distancia euclídea entre dos puntos.
    public static float Distance(Vector2 a, Vector2 b) => Vector2.Distance(a, b);

    // Dirección normalizada desde 'source' hacia 'target'.
    public static Vector2 Direction(Vector2 source, Vector2 target)
    {
        Vector2 dir = target - source;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.zero;
    }
}