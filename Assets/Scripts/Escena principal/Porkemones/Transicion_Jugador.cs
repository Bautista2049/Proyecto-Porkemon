using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
