using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// OBSOLETO: Reemplazado por TransicionEscena.cs (script unificado configurable desde el Inspector).
/// Para migrar: reemplaza este componente por TransicionEscena y configura nombreEscenaDestino = "Escena Gimnasio".
/// </summary>
[System.Obsolete("Usar TransicionEscena.cs en su lugar")]
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
