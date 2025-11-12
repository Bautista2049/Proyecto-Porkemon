using UnityEngine;
using UnityEngine.SceneManagement;

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