using System;

// Canal de mensajes neutral para puntaje.
// Quien recolecta notifica. Quien contabiliza escucha.
public static class ScoreEvents
{
    public static event Action<int> OnPuntosRecolectados;
    public static void Notificar(int cantidad) => OnPuntosRecolectados?.Invoke(cantidad);
}