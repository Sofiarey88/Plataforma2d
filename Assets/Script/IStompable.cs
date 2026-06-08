using UnityEngine;
// Contrato para entidades que el jugador puede eliminar pisándolas desde arriba.
public interface IStompable
{
    // Llamado por StompTrigger cuando el jugador aterriza encima.
    // Responsable de aplicar las consecuencias (muerte, destrucción, etc.).
    void OnStomp();
}