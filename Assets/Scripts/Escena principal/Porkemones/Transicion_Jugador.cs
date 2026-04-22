using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// OBSOLETO: Reemplazado por TransicionEscena.cs (script unificado configurable desde el Inspector).
/// Este script se mantiene temporalmente para no romper las referencias en escenas.
/// Para migrar: reemplaza este componente por TransicionEscena y configura nombreEscenaDestino = "Escena Jugador".
/// </summary>
[System.Obsolete("Usar TransicionEscena.cs en su lugar")]
public class Transicion_Jugador : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameState.posicionJugadorGuardadaDisponible = false;
            SceneManager.LoadScene("Escena CasaJ1");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }
}
