using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// OBSOLETO: Reemplazado por TransicionEscena.cs (script unificado configurable desde el Inspector).
/// Para migrar: reemplaza este componente por TransicionEscena y configura nombreEscenaDestino.
/// </summary>
[System.Obsolete("Usar TransicionEscena.cs en su lugar")]
public class Transicion_Vuelta : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Escena Principal";
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Transitioning to scene: {sceneToLoad}");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}