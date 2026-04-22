using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// OBSOLETO: Reemplazado por TransicionEscena.cs (script unificado configurable desde el Inspector).
/// Para migrar: reemplaza este componente por TransicionEscena, configura nombreEscenaDestino y activa activarModoTienda.
/// </summary>
[System.Obsolete("Usar TransicionEscena.cs en su lugar")]
public class Transicion_Tienda : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameState.modoTienda = true;
            GameState.posicionJugadorGuardadaDisponible = false;
            SceneManager.LoadScene("Escena TiendaInterior");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
