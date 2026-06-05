using UnityEngine;
/// <summary>
/// Contrato para entidades que el jugador puede eliminar pisándolas desde arriba.
/// Implementar en Enemy, Projectile o cualquier obstáculo interactuable al pisarlo.
/// </summary>
public interface IStompable
{
    /// <summary>
    /// Llamado por StompTrigger cuando el jugador aterriza encima.
    /// Responsable de aplicar las consecuencias (muerte, destrucción, etc.).
    /// </summary>
    void OnStomp();
}