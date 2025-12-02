using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transicion_Gimnasio : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameState.posicionJugadorGuardadaDisponible = false;
            SceneManager.LoadScene("Escena Gimnasio");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }
}
