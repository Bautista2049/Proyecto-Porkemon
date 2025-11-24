using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
