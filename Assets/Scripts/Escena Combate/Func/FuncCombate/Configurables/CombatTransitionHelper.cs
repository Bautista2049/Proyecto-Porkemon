using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Helper estático que centraliza la lógica común de transición a combate.
/// Usado por Transicion_Combate, TriggerBossGiratina y ControladorNPCEntrenador.
/// </summary>
public static class CombatTransitionHelper
{
    /// <summary>
    /// Calcula una posición segura de retorno para el jugador tras la colisión.
    /// </summary>
    public static Vector3 CalcularPosicionRetorno(Collision collision)
    {
        Vector3 posicion = collision.transform.position;

        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            normal.y = 0f;
            if (normal.sqrMagnitude > 0.0001f)
            {
                normal.Normalize();
                posicion += normal * 0.75f;
            }
        }

        return posicion;
    }

    /// <summary>
    /// Guarda el estado necesario y carga la escena de combate.
    /// </summary>
    public static void IniciarTransicionCombate(string nombreEscena)
    {
        if (GestorDeBatalla.instance != null)
            GestorDeBatalla.instance.combateIniciado = false;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            Object.DontDestroyOnLoad(mainCamera.gameObject);

        SceneManager.LoadScene(nombreEscena);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Obtiene la escena de combate correcta según si es boss o no.
    /// </summary>
    public static string GetEscenaCombate()
    {
        return GameState.esCombateBoss ? "EscenaCombateBoss" : "Escena de combate";
    }
}
