using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transicion_Tienda : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameState.posicionJugadorGuardadaDisponible = false;
            SceneManager.LoadScene("Escena Tienda");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
