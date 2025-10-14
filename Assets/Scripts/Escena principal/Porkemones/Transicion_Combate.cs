using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Transicion_Combate : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "Escena de Combate" ;
    [SerializeField] private GameObject popupPanel; 
    [SerializeField] private TextMeshProUGUI popupText; 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (GameState.porkemonDelBot != null && GameState.porkemonDelBot.VidaActual <= 0)
            {
                if (popupText != null)
                {
                    popupText.gameObject.SetActive(true);
                    popupText.text = "El Porkemon del bot está debilitado. No puedes iniciar un combate.";
                    StartCoroutine(DesactivarTexto(3f));
                }
                return;
            }

            SceneManager.LoadScene(nombreEscena);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private IEnumerator DesactivarTexto(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }
}
