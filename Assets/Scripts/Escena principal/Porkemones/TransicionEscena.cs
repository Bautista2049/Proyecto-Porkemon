using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script unificado para transiciones de escena simples.
/// Reemplaza: Transicion_Jugador, Transicion_Tienda, Transicion_Gimnasio y Transicion_Vuelta.
/// Configurable desde el Inspector para cada caso de uso.
/// </summary>
public class TransicionEscena : MonoBehaviour
{
    public enum MetodoDeteccion { Collision, Trigger }

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaDestino = "Escena Principal";
    [SerializeField] private MetodoDeteccion metodoDeteccion = MetodoDeteccion.Collision;

    [Header("Estado del Juego")]
    [Tooltip("Si se marca, resetea la posición guardada del jugador al transicionar.")]
    [SerializeField] private bool resetearPosicionGuardada = false;

    [Tooltip("Si se marca, activa el modo tienda en GameState.")]
    [SerializeField] private bool activarModoTienda = false;

    [Header("Cursor")]
    [SerializeField] private bool cursorVisible = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (metodoDeteccion != MetodoDeteccion.Collision) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        EjecutarTransicion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (metodoDeteccion != MetodoDeteccion.Trigger) return;
        if (!other.CompareTag("Player")) return;

        EjecutarTransicion();
    }

    private void EjecutarTransicion()
    {
        if (resetearPosicionGuardada)
            GameState.posicionJugadorGuardadaDisponible = false;

        if (activarModoTienda)
            GameState.modoTienda = true;

        SceneManager.LoadScene(nombreEscenaDestino);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = cursorVisible;
    }
}
